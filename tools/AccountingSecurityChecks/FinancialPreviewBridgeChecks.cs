using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyperyek.Accounting.Contracts;

internal static class FinancialPreviewBridgeChecks
{
    internal static IntegrationFinancialPreviewRequest Sample => new(7, "tenant-a", 10, "order", "billing-group", 1,
            "irr-preview-v1", IntegrationMoneyUnit.IRR, true,
            [new("line", 1, 10_000_000, 1_000_000, true)],
            0, 600_000, 0, 0, 500_000, 9_100_000, 900_000, 0, 0, 0, 9_600_000);

    internal static async Task RunAsync(HttpClient authenticatedHttp, Action<bool, string> check)
    {
        var request = Sample;
        var live = new AccountingFinancialPreviewClient(authenticatedHttp);
        var result = await live.PreviewAsync(request);
        check(result is { PreviewOnly: true, Status: IntegrationFinancialReviewStatus.Validated,
            InvoiceTotal: 9_600_000, BuyerPayable: 9_100_000, ExpectedSettlement: 8_700_000 }
            && result.Lines.Single().Net == 9_000_000 && result.Components?.PlatformCommission == 900_000,
            "Integration bridge -> real authorized preview controller -> mapped financial result (no SQL)");
        var unknown = await live.PreviewAsync(request with { PlatformCommission = null });
        check(unknown is { Status: IntegrationFinancialReviewStatus.AwaitingEvidence, InvoiceTotal: 9_600_000,
            ExpectedSettlement: null } && unknown.Components?.PlatformCommission is null
            && unknown.Issues.Any(x => x.Code == "AmountUnknown" && x.Field == "platformCommission"),
            "bridge preserves unknown commission, partial invoice and evidence issues");
        var mismatch = await live.PreviewAsync(request with { BuyerPayment = 1 });
        check(mismatch is { Status: IntegrationFinancialReviewStatus.NeedsReview, ExpectedSettlement: null }
            && mismatch.Issues.Count > 0, "HTTP 200 review result is not an applied business command");
        var unverified = await live.PreviewAsync(request with { SourceUnitVerified = false });
        check(unverified.Status == IntegrationFinancialReviewStatus.AwaitingEvidence && unverified.InvoiceTotal is null,
            "bridge does not promote unverified source currency to verified");
        var unknownUnit = await live.PreviewAsync(request with { SourceUnit = IntegrationMoneyUnit.Unknown });
        check(unknownUnit.Status == IntegrationFinancialReviewStatus.NeedsReview,
            "unknown unit is preserved for accounting validation, not defaulted to IRR");
        var toman = await live.PreviewAsync(request with { SourceUnit = IntegrationMoneyUnit.Toman });
        check(toman.InvoiceTotal == 96_000_000 && toman.ExpectedSettlement == 87_000_000,
            "Toman conversion occurs once in accounting, not again in the bridge");
        var allocations = await live.PreviewAsync(request with
        {
            Lines = [new("a", 2, 100, 10, true, 30), new("b", 1, 100, 0, false, 0)],
            SellerOrderDiscount = 30, MerchantShipping = 0, PlatformFunding = 0,
            BuyerPayment = 260, PlatformCommission = 0, ReportedInvoiceTotal = 260
        });
        check(allocations.Status == IntegrationFinancialReviewStatus.Validated
            && allocations.Lines.Single(x => x.LineId == "a").AllocatedOrderDiscount == 30
            && allocations.Lines.Single(x => x.LineId == "b").Net == 100,
            "explicit allocations and eligibility reach the calculator unchanged");

        var goodJson = JsonSerializer.Serialize(result, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var transport = new PreviewTransport { Body = goodJson };
        using var http = new HttpClient(transport) { BaseAddress = new Uri("https://accounting.fixture.invalid/") };
        var client = new AccountingFinancialPreviewClient(http);
        var evidence = request with
        {
            ShopId = 19, TenantId = "tenant-b", ConnectionId = 987, ExternalOrderId = "o/2",
            BillingGroupId = "parcel-group", SourceVersion = 42,
            SellerOrderDiscount = 11, MerchantShipping = 12, ThirdPartyShipping = 13,
            InvoiceTax = 14, PlatformFunding = 15, BuyerPayment = 16, PlatformCommission = null,
            CommissionTax = 18, SettlementDeductions = 19, SettlementCredits = 20, ReportedInvoiceTotal = 21,
            Lines = [new("line", 1.5m, 123.45m, null, false, 0)]
        };
        await client.PreviewAsync(evidence);
        var sent = transport.Request!;
        check(transport.Method == HttpMethod.Post && transport.Path == "/api/hyperyek/v2/accounting/financial-preview"
            && sent.Scope.ShopId == 19 && sent.Scope.TenantId == "tenant-b" && sent.ConnectionId == 987
            && sent.ExternalOrderId == "o/2" && sent.BillingGroupId == "parcel-group" && sent.SourceVersion == 42
            && sent.PolicyVersion == evidence.PolicyVersion && sent.SourceUnit == FinancialSourceUnit.IRR && sent.SourceUnitVerified,
            "mapper preserves all scope/version fields and calls only the v2 no-write route");
        check(sent.SellerOrderDiscount == 11 && sent.MerchantShipping == 12 && sent.ThirdPartyShipping == 13
            && sent.InvoiceTax == 14 && sent.PlatformFunding == 15 && sent.BuyerPayment == 16
            && sent.PlatformCommission is null && sent.CommissionTax == 18 && sent.SettlementDeductions == 19
            && sent.SettlementCredits == 20 && sent.ReportedInvoiceTotal == 21
            && sent.Lines.Single() == new FinancialPreviewLine("line", 1.5m, 123.45m, null, false, 0),
            "mapper preserves every amount, null, quantity, eligibility and explicit allocation");

        async Task Reject(Func<Task> action, string code, bool retryable)
        {
            try { await action(); }
            catch (IntegrationProviderException ex) when (ex.Code == code && ex.Retryable == retryable && ex.InnerException is null)
            { check(true, code + " retryable=" + retryable); return; }
            throw new Exception("Expected sanitized " + code);
        }
        foreach (var (status, retry) in new[] { (400, false), (401, false), (403, false), (404, false),
                     (409, false), (422, false), (408, true), (429, true), (500, true), (503, true), (204, false), (302, false) })
        {
            transport.Status = (HttpStatusCode)status; transport.Body = "sensitive-provider-error";
            var before = transport.Calls;
            await Reject(() => client.PreviewAsync(request), "AccountingPreviewHttp" + status, retry);
            check(transport.Calls == before + 1, "preview transport never automatically replays HTTP " + status);
        }
        transport.Status = HttpStatusCode.OK;
        foreach (var body in new[] { "null", "[]", "{}", "sensitive-invalid-json" })
        {
            transport.Body = body;
            await Reject(() => client.PreviewAsync(request), "AccountingPreviewResponseInvalid", false);
        }
        foreach (var field in new[] { "previewOnly", "status", "lines", "issues", "invoiceTotal" })
        {
            var body = JsonNode.Parse(goodJson)!; body.AsObject().Remove(field);
            transport.Body = body.ToJsonString();
            await Reject(() => client.PreviewAsync(request), "AccountingPreviewResponseInvalid", false);
        }
        foreach (var (field, value) in new (string, JsonNode?)[]
        { ("previewOnly", JsonValue.Create(false)), ("status", JsonValue.Create(99)), ("currency", JsonValue.Create("USD")),
          ("policyVersion", JsonValue.Create("future")), ("lines", JsonNode.Parse("[null]")) })
        {
            var body = JsonNode.Parse(goodJson)!; body[field] = value;
            transport.Body = body.ToJsonString();
            await Reject(() => client.PreviewAsync(request), "AccountingPreviewResponseInvalid", false);
        }
        transport.Body = goodJson;
        var callsBeforeInvalid = transport.Calls;
        await Reject(() => client.PreviewAsync(request with { SourceUnit = (IntegrationMoneyUnit)99 }), "AccountingPreviewInputInvalid", false);
        await Reject(() => client.PreviewAsync(request with { Lines = null! }), "AccountingPreviewInputInvalid", false);
        await Reject(() => client.PreviewAsync(request with { Lines = [] }), "AccountingPreviewInputInvalid", false);
        await Reject(() => client.PreviewAsync(request with { Lines = [null!] }), "AccountingPreviewInputInvalid", false);
        await Reject(() => client.PreviewAsync(request with { Lines = Enumerable.Repeat(request.Lines[0], 1001).ToArray() }),
            "AccountingPreviewInputInvalid", false);
        check(transport.Calls == callsBeforeInvalid, "invalid input rejected before HTTP");
        transport.Failure = new HttpRequestException("sensitive-url-or-token");
        await Reject(() => client.PreviewAsync(request), "AccountingPreviewUnavailable", true);
        transport.Failure = new TaskCanceledException("sensitive-timeout");
        await Reject(() => client.PreviewAsync(request), "AccountingPreviewTimeout", true);
        transport.Failure = null;
        using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
        try { await client.PreviewAsync(request, cancelled.Token); throw new Exception("Cancellation lost"); }
        catch (OperationCanceledException) { check(true, "caller cancellation stays cancellation rather than provider failure"); }
    }

    private sealed class PreviewTransport : HttpMessageHandler
    {
        internal int Calls;
        internal string Body = "";
        internal HttpStatusCode Status = HttpStatusCode.OK;
        internal Exception? Failure;
        internal FinancialPreviewRequest? Request;
        internal HttpMethod? Method;
        internal string? Path;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested(); Calls++; Method = request.Method; Path = request.RequestUri!.AbsolutePath;
            if (Failure is not null) throw Failure;
            Request = await request.Content!.ReadFromJsonAsync<FinancialPreviewRequest>(cancellationToken: ct);
            return new(Status) { Content = new StringContent(Body, System.Text.Encoding.UTF8, "application/json") };
        }
    }
}
