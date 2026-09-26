using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Hyper.Integration.Domain.Features.Integrations;
using Hyperyek.Accounting.Contracts;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Accounting boundary mapper. Contains no financial arithmetic or database access.</summary>
public sealed class AccountingFinancialPreviewClient(HttpClient client) : IIntegrationFinancialPreviewPort
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    private const string Route = "api/hyperyek/v2/accounting/financial-preview";

    public async Task<IntegrationFinancialPreviewResult> PreviewAsync(IntegrationFinancialPreviewRequest request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Lines is null || request.Lines.Count is 0 or > 1000 || request.Lines.Any(x => x is null))
            throw new IntegrationProviderException("AccountingPreviewInputInvalid", false);
        var unit = request.SourceUnit switch
        {
            IntegrationMoneyUnit.IRR => FinancialSourceUnit.IRR,
            IntegrationMoneyUnit.Toman => FinancialSourceUnit.Toman,
            IntegrationMoneyUnit.Unknown => FinancialSourceUnit.Unknown,
            _ => throw new IntegrationProviderException("AccountingPreviewInputInvalid", false)
        };
        var command = new FinancialPreviewRequest(new(request.ShopId, request.TenantId), request.ConnectionId,
            request.ExternalOrderId, request.BillingGroupId, request.SourceVersion, request.PolicyVersion,
            unit, request.SourceUnitVerified,
            request.Lines.Select(x => new FinancialPreviewLine(x.LineId, x.Quantity, x.UnitPrice,
                x.SellerDiscount, x.EligibleForOrderDiscount, x.ExplicitOrderDiscount)).ToArray(),
            request.SellerOrderDiscount, request.MerchantShipping, request.ThirdPartyShipping, request.InvoiceTax,
            request.PlatformFunding, request.BuyerPayment, request.PlatformCommission, request.CommissionTax,
            request.SettlementDeductions, request.SettlementCredits, request.ReportedInvoiceTotal);
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(ct);
        deadline.CancelAfter(client.Timeout);
        var operationToken = deadline.Token;
        try
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, Route) { Content = JsonContent.Create(command) };
            using var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, operationToken);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new IntegrationProviderException($"AccountingPreviewHttp{(int)response.StatusCode}",
                    response.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
                    || (int)response.StatusCode >= 500);
            // Bound the response and inspect the actual wire flag: the Contracts property is computed.
            await response.Content.LoadIntoBufferAsync(1_048_576, operationToken);
            using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(operationToken), cancellationToken: operationToken);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("previewOnly", out var flag)
                || flag.ValueKind != JsonValueKind.True)
                throw InvalidResponse();
            var result = root.Deserialize<FinancialPreviewResult>(Json);
            if (result is null || result.Currency != "IRR" || result.PolicyVersion != "irr-preview-v1"
                || !Enum.IsDefined(result.Status) || result.Lines is null || result.Issues is null
                || result.Lines.Count > request.Lines.Count
                || result.Lines.Any(x => x is null || !request.Lines.Any(y => y.LineId == x.LineId))
                || result.Lines.Select(x => x.LineId).Distinct(StringComparer.Ordinal).Count() != result.Lines.Count
                || result.Issues.Any(x => x is null || string.IsNullOrWhiteSpace(x.Code)
                    || string.IsNullOrWhiteSpace(x.Field) || x.Status is not (FinancialPreviewStatus.AwaitingEvidence or FinancialPreviewStatus.NeedsReview)))
                throw InvalidResponse();
            if (result.Status == FinancialPreviewStatus.Validated)
            {
                if (result.Issues.Count != 0 || result.Lines.Count != request.Lines.Count || result.Components is null
                    || result.Gross is null || result.SellerDiscount is null || result.NetGoods is null
                    || result.InvoiceTotal is null || result.BuyerPayable is null || result.ExpectedSettlement is null)
                    throw InvalidResponse();
            }
            else if (result.Issues.Count == 0 || result.Issues.Max(x => x.Status) != result.Status
                || result.ExpectedSettlement is not null) throw InvalidResponse();

            var parts = result.Components;
            return new(ToStatus(result.Status), result.PolicyVersion, result.Currency,
                result.Lines.Select(x => new IntegrationFinancialLineResult(x.LineId, x.Gross,
                    x.SellerDiscount, x.AllocatedOrderDiscount, x.Net)).ToArray(),
                result.Gross, result.SellerDiscount, result.NetGoods, result.InvoiceTotal,
                result.BuyerPayable, result.ExpectedSettlement,
                result.Issues.Select(x => new IntegrationFinancialIssue(x.Code, x.Field, ToStatus(x.Status))).ToArray(),
                parts is null ? null : new(parts.SellerOrderDiscount, parts.MerchantShipping, parts.ThirdPartyShipping,
                    parts.InvoiceTax, parts.PlatformFunding, parts.BuyerPayment, parts.PlatformCommission,
                    parts.CommissionTax, parts.SettlementDeductions, parts.SettlementCredits));
        }
        catch (JsonException) { throw InvalidResponse(); }
        catch (HttpRequestException) { throw new IntegrationProviderException("AccountingPreviewUnavailable", true); }
        catch (IOException) { throw new IntegrationProviderException("AccountingPreviewUnavailable", true); }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        { throw new IntegrationProviderException("AccountingPreviewTimeout", true); }
    }

    private static IntegrationProviderException InvalidResponse() => new("AccountingPreviewResponseInvalid", false);
    private static IntegrationFinancialReviewStatus ToStatus(FinancialPreviewStatus status) => status switch
    {
        FinancialPreviewStatus.Validated => IntegrationFinancialReviewStatus.Validated,
        FinancialPreviewStatus.AwaitingEvidence => IntegrationFinancialReviewStatus.AwaitingEvidence,
        FinancialPreviewStatus.NeedsReview => IntegrationFinancialReviewStatus.NeedsReview,
        _ => throw InvalidResponse()
    };
}
