using System.Text.Json;
using System.Text.Json.Serialization;
using Hyper.AdminPanel.Web.ViewModels;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.AdminPanel.Web.Infrastructure;

// Panel orchestration only: no provider requests, SQL mutations, queue or financial arithmetic.
public sealed class AdminFinancialPreviewWorkflow(IAdminMerchantSimulationService simulations,
    AdminSimulationTickets tickets, IIntegrationScenarioQueue scenarios, IIntegrationFinancialPreviewPort financial)
{
    public const string CookieName = "Hyper.AdminMerchantSimulation";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = false, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow, MaxDepth = 8
    };

    public async Task<FinancialPreviewViewModel> PrepareAsync(string adminId, string? cookie, CancellationToken ct)
    {
        try
        {
            var selected = await SelectedAsync(adminId, cookie, ct);
            if (selected is null) return new(null, [], new());
            var connections = await ConnectionsAsync(selected, ct);
            return new(selected, connections, new() { ContextTicket = tickets.Protect(adminId, selected.Id),
                ConnectionId = connections.FirstOrDefault()?.Id ?? 0 });
        }
        catch (IntegrationProviderException) { return ContextUnavailable(); }
        catch (HttpRequestException) { return ContextUnavailable(); }
        catch (InvalidOperationException)
        {
            return new(null, [], new(), Error: "تنظیمات سرویس خواندن زمینهٔ مغازه کامل نیست؛ مسئول سامانه باید آن را بررسی کند.", StatusCode: 503);
        }
    }

    public async Task<FinancialPreviewViewModel> PreviewAsync(string adminId, string? cookie,
        FinancialPreviewForm form, CancellationToken ct)
    {
        var posted = tickets.Read(adminId, form.ContextTicket);
        if (posted is null || tickets.Read(adminId, cookie) != posted)
            return new(null, [], new(), Error: "زمینه مغازه تغییر کرده است؛ صفحه را تازه کنید.", StatusCode: 409);
        var page = await PrepareAsync(adminId, cookie, ct);
        if (page.Error is not null) return page;
        if (page.Selected is null || page.Selected.Id != posted)
            return new(null, [], new(), Error: "زمینه شبیه‌سازی منقضی یا نامعتبر است؛ دوباره مغازه را انتخاب کنید.", StatusCode: 409);
        page = page with { Form = form };
        if (!page.Connections.Any(x => x.Id == form.ConnectionId))
            return page with { Error = "اتصال فعال باسلام متعلق به مغازه انتخاب‌شده نیست.", StatusCode = 409 };
        if (string.IsNullOrWhiteSpace(form.DraftJson) || form.DraftJson.Length > 100_000)
            return page with { Error = "ورودی مالی خالی یا بیش از حد مجاز است.", StatusCode = 400 };

        FinancialPreviewDraft? draft;
        try
        {
            using var document = JsonDocument.Parse(form.DraftJson, new JsonDocumentOptions { MaxDepth = 8 });
            if (HasDuplicateFields(document.RootElement)) throw new JsonException();
            draft = document.RootElement.Deserialize<FinancialPreviewDraft>(Json);
            if (draft?.Lines is null || draft.Lines.Count is 0 or > 1000 || draft.Lines.Any(x => x is null)
                || !Enum.IsDefined(draft.SourceUnit))
                throw new JsonException();
        }
        catch (JsonException)
        {
            return page with { Error = "JSON معتبر با ۱ تا ۱۰۰۰ قلم وارد کنید. فیلد ناشناخته، تکراری یا شناسهٔ زمینه در JSON مجاز نیست.", StatusCode = 400 };
        }
        try
        {
            var result = await financial.PreviewAsync(draft.ToRequest(page.Selected, form.ConnectionId, form.UnitAcknowledged), ct);
            // Do not display a result if the simulation expired/was ended during the network call.
            var current = await SelectedAsync(adminId, cookie, ct);
            if (current is null || current.Id != page.Selected.Id || current.ShopId != page.Selected.ShopId
                || current.TenantId != page.Selected.TenantId)
                return new(null, [], new(), Error: "زمینه شبیه‌سازی هنگام محاسبه تغییر کرد؛ دوباره انتخاب کنید.", StatusCode: 409);
            return page with { Result = result };
        }
        catch (IntegrationProviderException ex)
        {
            return page with { Error = ex.Retryable
                ? "سرویس حسابداری موقتاً در دسترس نیست؛ بعداً دوباره پیش‌نمایش بگیرید."
                : "پیش‌نمایش پذیرفته نشد؛ تنظیمات دسترسی سرویس یا ورودی را بررسی کنید.", StatusCode = 502 };
        }
        catch (InvalidOperationException)
        {
            return page with { Error = "تنظیمات اتصال امن به API حسابداری کامل نیست؛ مسئول سامانه باید آن را بررسی کند.", StatusCode = 503 };
        }
        catch (HttpRequestException) { return ContextUnavailable(); }
    }

    private static FinancialPreviewViewModel ContextUnavailable() => new(null, [], new(),
        Error: "سرویس خواندن زمینهٔ مغازه در دسترس نیست؛ کمی بعد دوباره تلاش کنید.", StatusCode: 502);

    private async Task<IntegrationAdminSimulation?> SelectedAsync(string adminId, string? cookie, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(adminId) || tickets.Read(adminId, cookie) is not { } id) return null;
        var selected = await simulations.GetAsync(adminId, id, ct);
        return selected is not null && selected.Id == id && !string.IsNullOrWhiteSpace(selected.TenantId)
            && AdminSimulationBoundary.Allows(selected, adminId, DateTime.UtcNow) ? selected : null;
    }

    private async Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(IntegrationAdminSimulation selected, CancellationToken ct) =>
        (await scenarios.ConnectionsAsync(new(selected.ShopId, selected.TenantId), ct))
            .Where(x => x.Provider == IntegrationProvider.Basalam && x.IsEnabled).ToArray();

    private static bool HasDuplicateFields(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var field in element.EnumerateObject())
                if (!names.Add(field.Name) || HasDuplicateFields(field.Value)) return true;
        }
        else if (element.ValueKind == JsonValueKind.Array)
            foreach (var child in element.EnumerateArray()) if (HasDuplicateFields(child)) return true;
        return false;
    }
}
