using System.Net.Http.Json;
using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface IWalletService
{
    Task<JsonElement> GetBalanceAsync(int userId, IReadOnlyCollection<object>? filters = null, int? operatorId = null, CancellationToken ct = default);
    Task<JsonElement> GetTransactionsAsync(int userId, int page = 1, int perPage = 50, int? operatorId = null, CancellationToken ct = default);
    Task<JsonElement> CreateExpenseAsync(int userId, object request, int? operatorId = null, CancellationToken ct = default);
    Task<JsonElement> GetExpenseAsync(int userId, int expenseId, int? operatorId = null, CancellationToken ct = default);
    Task<JsonElement> DeleteExpenseAsync(int userId, int expenseId, int rollbackReasonId, int? operatorId = null, CancellationToken ct = default);
    Task<JsonElement> GetExpenseByReferenceAsync(int userId, int reasonId, int referenceId, int? operatorId = null, CancellationToken ct = default);
    Task<JsonElement> DeleteExpenseByReferenceAsync(int userId, int reasonId, int referenceId, int rollbackReasonId, int? operatorId = null, CancellationToken ct = default);
}

public sealed class WalletService(IBasalamHttpClient client) : IWalletService
{
    public Task<JsonElement> GetBalanceAsync(int userId, IReadOnlyCollection<object>? filters = null, int? operatorId = null, CancellationToken ct = default)
    {
        var payloadFilters = filters is { Count: > 0 } ? filters.ToArray() : new object[] { Array.Empty<object>() };
        return SendAsync(HttpMethod.Post, $"/v1/users/{Id(userId, "userId")}/balance",
            new { filters = payloadFilters }, operatorId, ct);
    }

    public Task<JsonElement> GetTransactionsAsync(int userId, int page = 1, int perPage = 50, int? operatorId = null, CancellationToken ct = default)
    {
        Id(userId, "userId");
        if (page < 1 || perPage is < 1 or > 100) throw Invalid("pagination");
        return SendAsync(HttpMethod.Get, $"/v1/users/{userId}/transactions?page={page}&per_page={perPage}", null, operatorId, ct);
    }

    public Task<JsonElement> CreateExpenseAsync(int userId, object request, int? operatorId = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return SendAsync(HttpMethod.Post, $"/v1/users/{Id(userId, "userId")}/expenses", request, operatorId, ct);
    }
    public Task<JsonElement> GetExpenseAsync(int userId, int expenseId, int? operatorId = null, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Get, $"/v1/users/{Id(userId, "userId")}/expenses/{Id(expenseId, "expenseId")}", null, operatorId, ct);
    public Task<JsonElement> DeleteExpenseAsync(int userId, int expenseId, int rollbackReasonId, int? operatorId = null, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Delete, $"/v1/users/{Id(userId, "userId")}/expenses/{Id(expenseId, "expenseId")}", new { rollback_reason_id = Id(rollbackReasonId, "rollbackReasonId") }, operatorId, ct);
    public Task<JsonElement> GetExpenseByReferenceAsync(int userId, int reasonId, int referenceId, int? operatorId = null, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Get, $"/v1/users/{Id(userId, "userId")}/expenses/by-ref/{Id(reasonId, "reasonId")}/{Id(referenceId, "referenceId")}", null, operatorId, ct);
    public Task<JsonElement> DeleteExpenseByReferenceAsync(int userId, int reasonId, int referenceId, int rollbackReasonId, int? operatorId = null, CancellationToken ct = default) =>
        SendAsync(HttpMethod.Delete, $"/v1/users/{Id(userId, "userId")}/expenses/by-ref/{Id(reasonId, "reasonId")}/{Id(referenceId, "referenceId")}", new { rollback_reason_id = Id(rollbackReasonId, "rollbackReasonId") }, operatorId, ct);

    private async Task<JsonElement> SendAsync(HttpMethod method, string path, object? body, int? operatorId, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, path);
        if (operatorId is not null) request.Headers.TryAddWithoutValidation("X-Operator-Id", operatorId.Value.ToString());
        if (body is not null) request.Content = JsonContent.Create(body);
        using var response = await client.SendAsync(request, ct);
        var text = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new BasalamAPIError($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}", (int)response.StatusCode, text);
        return string.IsNullOrWhiteSpace(text) ? default : JsonDocument.Parse(text).RootElement.Clone();
    }
    private static int Id(int value, string field)
    {
        if (value <= 0) throw Invalid(field);
        return value;
    }
    private static BasalamValidationError Invalid(string field) =>
        new(new Dictionary<string, IReadOnlyList<string>> { [field] = ["Value is invalid"] });
}
