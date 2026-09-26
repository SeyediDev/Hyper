using System.ComponentModel.DataAnnotations;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.AdminPanel.Web.ViewModels;

public sealed class FinancialPreviewForm
{
    [Required, StringLength(2048)] public string ContextTicket { get; set; } = "";
    [Range(1, long.MaxValue)] public long ConnectionId { get; set; }
    [Required, StringLength(100_000)] public string DraftJson { get; set; } = FinancialPreviewDraft.Sample;
    public bool UnitAcknowledged { get; set; }
}

public sealed record FinancialPreviewViewModel(IntegrationAdminSimulation? Selected,
    IReadOnlyList<IntegrationScenarioConnection> Connections, FinancialPreviewForm Form,
    IntegrationFinancialPreviewResult? Result = null, string? Error = null, int StatusCode = 200);

// Deliberately no shop, tenant, connection or source-verification fields in editable JSON.
public sealed record FinancialPreviewDraft(string ExternalOrderId, string BillingGroupId, long SourceVersion,
    IntegrationMoneyUnit SourceUnit, IReadOnlyList<IntegrationFinancialLine>? Lines,
    decimal? SellerOrderDiscount, decimal? MerchantShipping, decimal? ThirdPartyShipping,
    decimal? InvoiceTax, decimal? PlatformFunding, decimal? BuyerPayment,
    decimal? PlatformCommission, decimal? CommissionTax, decimal? SettlementDeductions,
    decimal? SettlementCredits, decimal? ReportedInvoiceTotal = null)
{
    public IntegrationFinancialPreviewRequest ToRequest(IntegrationAdminSimulation selected, long connectionId, bool unitAcknowledged) =>
        new(selected.ShopId, selected.TenantId, connectionId, ExternalOrderId, BillingGroupId, SourceVersion,
            "irr-preview-v1", SourceUnit, unitAcknowledged, Lines!, SellerOrderDiscount, MerchantShipping,
            ThirdPartyShipping, InvoiceTax, PlatformFunding, BuyerPayment, PlatformCommission, CommissionTax,
            SettlementDeductions, SettlementCredits, ReportedInvoiceTotal);

    public const string Sample = """
        {
          "externalOrderId": "manual-example",
          "billingGroupId": "manual-billing-group",
          "sourceVersion": 1,
          "sourceUnit": 1,
          "lines": [
            { "lineId": "one", "quantity": 1, "unitPrice": 10000000,
              "sellerDiscount": 1000000, "eligibleForOrderDiscount": true }
          ],
          "sellerOrderDiscount": 0,
          "merchantShipping": 600000,
          "thirdPartyShipping": 0,
          "invoiceTax": 0,
          "platformFunding": 500000,
          "buyerPayment": 9100000,
          "platformCommission": 900000,
          "commissionTax": 0,
          "settlementDeductions": 0,
          "settlementCredits": 0,
          "reportedInvoiceTotal": 9600000
        }
        """;
}
