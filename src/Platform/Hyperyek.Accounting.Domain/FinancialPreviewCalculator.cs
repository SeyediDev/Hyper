using System.Numerics;
using Hyperyek.Accounting.Contracts;

namespace Hyperyek.Accounting.Domain;

/// <summary>Pure, bounded arithmetic for one merchant billing group. No persistence or provider calls.</summary>
public sealed class FinancialPreviewCalculator
{
    public const string PolicyVersion = "irr-preview-v1";
    private const decimal Limit = 1_000_000_000_000_000_000m;

    public FinancialPreviewResult Preview(FinancialPreviewRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var issues = new List<FinancialPreviewIssue>();
        FinancialPreviewComponents? components = null;
        void Review(string code, string field) => issues.Add(new(code, field, FinancialPreviewStatus.NeedsReview));
        FinancialPreviewResult Result(IReadOnlyList<FinancialPreviewLineResult>? lines = null,
            decimal? gross = null, decimal? discount = null, decimal? net = null, decimal? invoice = null,
            decimal? buyer = null, decimal? settlement = null) => new(
                issues.Count == 0 ? FinancialPreviewStatus.Validated : issues.Max(x => x.Status),
                PolicyVersion, "IRR", lines ?? [], gross, discount, net, invoice, buyer, settlement, issues.ToArray(), components);

        if (request.Scope is null || request.Scope.ShopId <= 0 || !Identity(request.Scope.TenantId, 30)
            || request.ConnectionId <= 0 || !Identity(request.ExternalOrderId, 128)
            || !Identity(request.BillingGroupId, 128) || request.SourceVersion <= 0)
            Review("InvalidFinancialScope", "scope");
        if (request.PolicyVersion != PolicyVersion) Review("UnsupportedFinancialPolicy", "policyVersion");
        if (request.SourceUnit is not (FinancialSourceUnit.IRR or FinancialSourceUnit.Toman))
            Review("UnsupportedCurrencyUnit", "sourceUnit");
        if (!request.SourceUnitVerified)
            issues.Add(new("CurrencyUnitUnverified", "sourceUnitVerified", FinancialPreviewStatus.AwaitingEvidence));
        if (request.Lines is null || request.Lines.Count is 0 or > 1000
            || request.Lines.Any(x => x is null || !Identity(x.LineId, 128))
            || request.Lines.Select(x => x.LineId).Distinct(StringComparer.Ordinal).Count() != request.Lines.Count)
            Review("InvalidFinancialLines", "lines");
        if (request.Lines is null || issues.Count != 0) return Result();

        var factor = request.SourceUnit == FinancialSourceUnit.Toman ? 10m : 1m;
        decimal? Money(decimal? value, string field, bool unitPrice = false)
        {
            if (value is null)
            {
                issues.Add(new("AmountUnknown", field, FinancialPreviewStatus.AwaitingEvidence));
                return null;
            }
            if (value < 0 || value >= Limit / factor)
            {
                Review("AmountOutOfRange", field); return null;
            }
            var normalized = value.Value * factor;
            if (decimal.Round(normalized, unitPrice ? 2 : 0) != normalized)
            {
                Review("AmountPrecisionUnsupported", field); return null;
            }
            return normalized;
        }

        try
        {
            var discount = Money(request.SellerOrderDiscount, "sellerOrderDiscount");
            var shipping = Money(request.MerchantShipping, "merchantShipping");
            var thirdParty = Money(request.ThirdPartyShipping, "thirdPartyShipping");
            var tax = Money(request.InvoiceTax, "invoiceTax");
            var funding = Money(request.PlatformFunding, "platformFunding");
            var paid = Money(request.BuyerPayment, "buyerPayment");
            var commission = Money(request.PlatformCommission, "platformCommission");
            var commissionTax = Money(request.CommissionTax, "commissionTax");
            var deductions = Money(request.SettlementDeductions, "settlementDeductions");
            var credits = Money(request.SettlementCredits, "settlementCredits");
            var reported = request.ReportedInvoiceTotal is null ? null : Money(request.ReportedInvoiceTotal, "reportedInvoiceTotal");
            components = new(discount, shipping, thirdParty, tax, funding, paid, commission, commissionTax, deductions, credits);
            var prepared = new List<PreparedLine>();
            foreach (var line in request.Lines.OrderBy(x => x.LineId, StringComparer.Ordinal))
            {
                var price = Money(line.UnitPrice, $"lines/{line.LineId}/unitPrice", true);
                var direct = Money(line.SellerDiscount, $"lines/{line.LineId}/sellerDiscount");
                if (line.Quantity <= 0 || line.Quantity > 1_000_000m || decimal.Round(line.Quantity, 3) != line.Quantity)
                {
                    Review("InvalidQuantity", $"lines/{line.LineId}/quantity"); continue;
                }
                if (price is null || direct is null) continue;
                var gross = Bounded(decimal.Round(line.Quantity * price.Value, 0, MidpointRounding.AwayFromZero));
                if (direct > gross) { Review("DiscountExceedsLine", $"lines/{line.LineId}"); continue; }
                prepared.Add(new(line, gross, direct.Value));
            }
            if (prepared.Count != request.Lines.Count || discount is null) return Result();
            var eligible = prepared.Where(x => x.Input.EligibleForOrderDiscount).ToArray();
            var capacity = Bounded(eligible.Sum(x => x.Gross - x.Direct));
            if (discount > capacity) { Review("DiscountExceedsEligibleTotal", "sellerOrderDiscount"); return Result(); }
            var allocations = prepared.ToDictionary(x => x.Input.LineId, _ => 0m, StringComparer.Ordinal);
            if (prepared.Any(x => x.Input.ExplicitOrderDiscount is not null))
            {
                // Partial explicit allocation is not permission to invent the remaining shares.
                foreach (var line in prepared)
                {
                    var share = Money(line.Input.ExplicitOrderDiscount, $"lines/{line.Input.LineId}/explicitOrderDiscount");
                    if (share is null) continue;
                    if (share > line.Gross - line.Direct || (!line.Input.EligibleForOrderDiscount && share != 0))
                        Review("InvalidExplicitAllocation", $"lines/{line.Input.LineId}");
                    allocations[line.Input.LineId] = share.Value;
                }
                if (prepared.Any(x => x.Input.ExplicitOrderDiscount is null)) return Result();
                if (allocations.Values.Sum() != discount)
                    Review("AllocationTotalMismatch", "sellerOrderDiscount");
                if (issues.Any(x => x.Status == FinancialPreviewStatus.NeedsReview)) return Result();
            }
            else if (discount > 0)
            {
                // Exact integer division prevents decimal multiplication overflow and lost remainder.
                var remainderOrder = new List<(string Id, BigInteger Remainder)>();
                foreach (var line in eligible)
                {
                    var numerator = new BigInteger(discount.Value) * new BigInteger(line.Gross - line.Direct);
                    var share = BigInteger.DivRem(numerator, new BigInteger(capacity), out var remainder);
                    allocations[line.Input.LineId] = (decimal)share;
                    remainderOrder.Add((line.Input.LineId, remainder));
                }
                var remaining = (int)(discount.Value - allocations.Values.Sum());
                foreach (var item in remainderOrder.OrderByDescending(x => x.Remainder)
                    .ThenBy(x => x.Id, StringComparer.Ordinal).Take(remaining)) allocations[item.Id]++;
            }

            var lines = prepared.Select(x => new FinancialPreviewLineResult(x.Input.LineId, x.Gross, x.Direct,
                allocations[x.Input.LineId], x.Gross - x.Direct - allocations[x.Input.LineId])).ToArray();
            var totalGross = Bounded(lines.Sum(x => x.Gross));
            var totalDiscount = Bounded(lines.Sum(x => x.SellerDiscount + x.AllocatedOrderDiscount));
            var net = Bounded(lines.Sum(x => x.Net));
            decimal? invoice = shipping is not null && tax is not null ? Bounded(net + shipping.Value + tax.Value) : null;
            if (invoice is not null && reported is not null && invoice != reported)
                Review("InvoiceTotalMismatch", "reportedInvoiceTotal");
            decimal? buyer = null;
            if (invoice is not null && funding is not null && thirdParty is not null)
            {
                if (funding > invoice) Review("FundingExceedsInvoice", "platformFunding");
                else buyer = Bounded(invoice.Value - funding.Value + thirdParty.Value);
            }
            if (buyer is not null && paid is not null && paid != buyer)
                Review("BuyerPaymentMismatch", "buyerPayment");
            decimal? settlement = null;
            if (issues.Count == 0 && invoice is not null && buyer == paid && commission is not null
                && commissionTax is not null && deductions is not null && credits is not null)
            {
                var expected = invoice.Value - commission.Value - commissionTax.Value - deductions.Value + credits.Value;
                if (expected < 0) Review("NegativeSettlementNeedsReview", "expectedSettlement");
                else settlement = Bounded(expected);
            }
            return Result(lines, totalGross, totalDiscount, net, invoice, buyer, settlement);
        }
        catch (OverflowException)
        {
            Review("FinancialAmountOverflow", "amounts");
            return Result();
        }
    }

    private static bool Identity(string? value, int length) => !string.IsNullOrWhiteSpace(value)
        && value.Length <= length && value == value.Trim() && !value.Any(char.IsControl);
    private static decimal Bounded(decimal value) => value >= 0 && value < Limit ? value : throw new OverflowException();
    private sealed record PreparedLine(FinancialPreviewLine Input, decimal Gross, decimal Direct);
}
