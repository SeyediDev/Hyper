namespace Hyper.Integration.Domain.Features.Integrations;

// Provider-neutral input evidence. Null money is unknown, never an implied zero.
// The accounting API owns calculation policy; Integration must not duplicate it.
public enum IntegrationMoneyUnit { Unknown = 0, IRR = 1, Toman = 2 }
public enum IntegrationFinancialReviewStatus { Validated = 1, AwaitingEvidence = 2, NeedsReview = 3 }

public sealed record IntegrationFinancialLine(string LineId, decimal Quantity, decimal? UnitPrice,
    decimal? SellerDiscount, bool EligibleForOrderDiscount, decimal? ExplicitOrderDiscount = null);

public sealed record IntegrationFinancialPreviewRequest(
    int ShopId, string TenantId, long ConnectionId, string ExternalOrderId, string BillingGroupId,
    long SourceVersion, string PolicyVersion, IntegrationMoneyUnit SourceUnit, bool SourceUnitVerified,
    IReadOnlyList<IntegrationFinancialLine> Lines,
    decimal? SellerOrderDiscount, decimal? MerchantShipping, decimal? ThirdPartyShipping,
    decimal? InvoiceTax, decimal? PlatformFunding, decimal? BuyerPayment,
    decimal? PlatformCommission, decimal? CommissionTax, decimal? SettlementDeductions,
    decimal? SettlementCredits, decimal? ReportedInvoiceTotal = null);

public sealed record IntegrationFinancialIssue(string Code, string Field, IntegrationFinancialReviewStatus Status);
public sealed record IntegrationFinancialLineResult(string LineId, decimal Gross,
    decimal SellerDiscount, decimal AllocatedOrderDiscount, decimal Net);
public sealed record IntegrationFinancialComponents(decimal? SellerOrderDiscount, decimal? MerchantShipping,
    decimal? ThirdPartyShipping, decimal? InvoiceTax, decimal? PlatformFunding, decimal? BuyerPayment,
    decimal? PlatformCommission, decimal? CommissionTax, decimal? SettlementDeductions, decimal? SettlementCredits);

public sealed record IntegrationFinancialPreviewResult(
    IntegrationFinancialReviewStatus Status, string PolicyVersion, string Currency,
    IReadOnlyList<IntegrationFinancialLineResult> Lines,
    decimal? Gross, decimal? SellerDiscount, decimal? NetGoods, decimal? InvoiceTotal,
    decimal? BuyerPayable, decimal? ExpectedSettlement,
    IReadOnlyList<IntegrationFinancialIssue> Issues, IntegrationFinancialComponents? Components)
{
    public bool PreviewOnly => true;
}

public interface IIntegrationFinancialPreviewPort
{
    // Caller supplies normalized evidence, not an unverified webhook body.
    // This is neither source/tenant authorization nor permission to post.
    Task<IntegrationFinancialPreviewResult> PreviewAsync(IntegrationFinancialPreviewRequest request,
        CancellationToken ct = default);
}
