namespace Hyperyek.Accounting.Contracts;

public enum FinancialPreviewStatus { Validated = 1, AwaitingEvidence = 2, NeedsReview = 3 }
public enum FinancialSourceUnit { Unknown = 0, IRR = 1, Toman = 2 }

// All input money uses SourceUnit. Null means unknown, never zero.
public sealed record FinancialPreviewLine(string LineId, decimal Quantity, decimal? UnitPrice,
    decimal? SellerDiscount, bool EligibleForOrderDiscount,
    decimal? ExplicitOrderDiscount = null);

public sealed record FinancialPreviewRequest(
    AccountingScope Scope, long ConnectionId, string ExternalOrderId, string BillingGroupId,
    long SourceVersion, string PolicyVersion, FinancialSourceUnit SourceUnit, bool SourceUnitVerified,
    IReadOnlyList<FinancialPreviewLine> Lines,
    decimal? SellerOrderDiscount, decimal? MerchantShipping, decimal? ThirdPartyShipping,
    decimal? InvoiceTax, decimal? PlatformFunding, decimal? BuyerPayment,
    decimal? PlatformCommission, decimal? CommissionTax, decimal? SettlementDeductions,
    decimal? SettlementCredits, decimal? ReportedInvoiceTotal = null);

public sealed record FinancialPreviewIssue(string Code, string Field, FinancialPreviewStatus Status);
public sealed record FinancialPreviewLineResult(string LineId, decimal Gross,
    decimal SellerDiscount, decimal AllocatedOrderDiscount, decimal Net);
public sealed record FinancialPreviewComponents(decimal? SellerOrderDiscount, decimal? MerchantShipping,
    decimal? ThirdPartyShipping, decimal? InvoiceTax, decimal? PlatformFunding, decimal? BuyerPayment,
    decimal? PlatformCommission, decimal? CommissionTax, decimal? SettlementDeductions, decimal? SettlementCredits);

public sealed record FinancialPreviewResult(
    FinancialPreviewStatus Status, string PolicyVersion, string Currency,
    IReadOnlyList<FinancialPreviewLineResult> Lines,
    decimal? Gross, decimal? SellerDiscount, decimal? NetGoods, decimal? InvoiceTotal,
    decimal? BuyerPayable, decimal? ExpectedSettlement,
    IReadOnlyList<FinancialPreviewIssue> Issues, FinancialPreviewComponents? Components = null)
{
    // Calculation validation is never authorization to post an invoice or journal.
    public bool PreviewOnly => true;
}
