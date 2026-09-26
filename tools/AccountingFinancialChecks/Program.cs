using System.Globalization;
using System.Text.Json;
using Hyperyek.Accounting.Contracts;
using Hyperyek.Accounting.Domain;

var calculator = new FinancialPreviewCalculator();
var checks = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception(name); Console.WriteLine("PASS " + name); checks++; }
var sample = new FinancialPreviewRequest(new(7, "tenant-a"), 10, "order", "merchant-parcel-group", 1,
    FinancialPreviewCalculator.PolicyVersion, FinancialSourceUnit.IRR, true,
    [new("one", 1, 10_000_000, 1_000_000, true)], 0, 600_000, 0, 0, 500_000, 9_100_000,
    900_000, 0, 0, 0, 9_600_000);
FinancialPreviewResult Preview(FinancialPreviewRequest value) => calculator.Preview(value);
bool Has(FinancialPreviewRequest value, string code) => Preview(value).Issues.Any(x => x.Code == code);
var result = Preview(sample);
Check(result.Status == FinancialPreviewStatus.Validated && result.PreviewOnly && result.Currency == "IRR"
    && result.InvoiceTotal == 9_600_000 && result.BuyerPayable == 9_100_000 && result.ExpectedSettlement == 8_700_000,
    "approved design example keeps invoice buyer and settlement distinct");
Check(result.Gross == 10_000_000 && result.SellerDiscount == 1_000_000 && result.NetGoods == 9_000_000,
    "platform funding and commission do not reduce goods sales");
var unknownFee = Preview(sample with { PlatformCommission = null });
Check(unknownFee.Status == FinancialPreviewStatus.AwaitingEvidence && unknownFee.InvoiceTotal == 9_600_000
    && unknownFee.ExpectedSettlement is null, "unknown fee keeps invoice calculation but withholds settlement");
Check(Preview(sample with { PlatformCommission = 0 }).ExpectedSettlement == 9_600_000,
    "explicit zero commission is distinct from unknown");
Check(Preview(sample with { InvoiceTax = null }).InvoiceTotal is null, "unknown tax cannot become zero");
Check(Preview(sample with { PlatformFunding = null }).BuyerPayable is null, "unknown funding cannot become seller discount");
var thirdParty = Preview(sample with { ThirdPartyShipping = 200_000, BuyerPayment = 9_300_000 });
Check(thirdParty.Status == FinancialPreviewStatus.Validated && thirdParty.InvoiceTotal == 9_600_000
    && thirdParty.BuyerPayable == 9_300_000 && thirdParty.ExpectedSettlement == 8_700_000,
    "direct third-party freight affects buyer payment not merchant invoice or settlement");
Check(Has(sample with { BuyerPayment = 9_600_000 }, "BuyerPaymentMismatch")
    && Preview(sample with { BuyerPayment = 9_600_000 }).ExpectedSettlement is null,
    "payment mismatch cannot authorize settlement");
Check(Has(sample with { ReportedInvoiceTotal = 1 }, "InvoiceTotalMismatch"), "source invoice mismatch requires review");
Check(Has(sample with { PlatformFunding = 10_000_000 }, "FundingExceedsInvoice"), "funding cannot exceed invoice");
Check(Has(sample with { PlatformCommission = 10_000_000 }, "NegativeSettlementNeedsReview"),
    "negative settlement is explicit review not clamped zero");
Check(Preview(sample with { CommissionTax = 100_000, SettlementDeductions = 200_000, SettlementCredits = 50_000 })
    .ExpectedSettlement == 8_450_000, "fee tax deductions and credits affect only settlement");
var toman = sample with { SourceUnit = FinancialSourceUnit.Toman,
    Lines = [new("one", 1, 1_000_000, 100_000, true)], MerchantShipping = 60_000,
    PlatformFunding = 50_000, BuyerPayment = 910_000, PlatformCommission = 90_000, ReportedInvoiceTotal = 960_000 };
Check(JsonSerializer.Serialize(Preview(toman)) == JsonSerializer.Serialize(result), "explicit toman normalizes exactly once to IRR");
Check(Has(sample with { SourceUnit = FinancialSourceUnit.Unknown }, "UnsupportedCurrencyUnit"), "missing currency is not inferred");
Check(Has(sample with { SourceUnitVerified = false }, "CurrencyUnitUnverified")
    && Preview(sample with { SourceUnitVerified = false }).InvoiceTotal is null, "unverified currency yields no calculated totals");
Check(Has(sample with { PolicyVersion = "other" }, "UnsupportedFinancialPolicy"), "policy version must be exact");
Check(Has(sample with { MerchantShipping = -1 }, "AmountOutOfRange"), "negative amount rejected");
Check(Has(sample with { MerchantShipping = .1m }, "AmountPrecisionUnsupported"), "fractional final IRR is not silently rounded");
Check(Has(sample with { Scope = null! }, "InvalidFinancialScope")
    && Has(sample with { Scope = new(0, "tenant-a") }, "InvalidFinancialScope")
    && Has(sample with { BillingGroupId = " " }, "InvalidFinancialScope"), "scope and billing group required");
Check(Has(sample with { Lines = null! }, "InvalidFinancialLines")
    && Has(sample with { Lines = [] }, "InvalidFinancialLines")
    && Has(sample with { Lines = [null!] }, "InvalidFinancialLines"), "null empty and null-element lines are handled");
Check(Has(sample with { Lines = [sample.Lines[0], sample.Lines[0]] }, "InvalidFinancialLines"), "duplicate line identity rejected");
Check(Has(sample with { Lines = Enumerable.Repeat(sample.Lines[0], 1001).ToArray() }, "InvalidFinancialLines"),
    "line count bounded");
Check(Has(sample with { Lines = [sample.Lines[0] with { Quantity = 0 }] }, "InvalidQuantity")
    && Has(sample with { Lines = [sample.Lines[0] with { Quantity = .0001m }] }, "InvalidQuantity"), "quantity range and precision checked");
Check(Preview(sample with { Lines = [sample.Lines[0] with { UnitPrice = null }] }).InvoiceTotal is null,
    "unknown line price never creates a subtotal");
Check(Has(sample with { Lines = [sample.Lines[0] with { SellerDiscount = 11_000_000 }] }, "DiscountExceedsLine"),
    "line discount bounded by gross");

var allocation = sample with { Lines = [new("c", 1, 1, 0, true), new("b", 1, 1, 0, true), new("a", 1, 1, 0, true)],
    SellerOrderDiscount = 1, MerchantShipping = 0, PlatformFunding = 0, BuyerPayment = 2,
    PlatformCommission = 0, ReportedInvoiceTotal = 2 };
var allocated = Preview(allocation);
Check(allocated.Status == FinancialPreviewStatus.Validated && allocated.Lines[0].LineId == "a"
    && allocated.Lines[0].AllocatedOrderDiscount == 1 && allocated.Lines.Sum(x => x.AllocatedOrderDiscount) == 1,
    "one rial across three lines uses deterministic largest remainder");
Check(JsonSerializer.Serialize(allocated) == JsonSerializer.Serialize(Preview(allocation with { Lines = allocation.Lines.Reverse().ToArray() })),
    "input line order cannot change allocation or output");
var weighted = allocation with { Lines = [new("a", 1, 100, 90, true), new("b", 1, 10, 0, true), new("c", 1, 100, 0, false)],
    SellerOrderDiscount = 3, BuyerPayment = 117, ReportedInvoiceTotal = 117 };
var weightedResult = Preview(weighted);
Check(weightedResult.Status == FinancialPreviewStatus.Validated
    && weightedResult.Lines[0].AllocatedOrderDiscount == 2 && weightedResult.Lines[1].AllocatedOrderDiscount == 1
    && weightedResult.Lines[2].AllocatedOrderDiscount == 0, "allocation uses net eligible weights not gross or excluded lines");
var explicitShares = allocation with { Lines = [new("a", 1, 1, 0, true, 0), new("b", 1, 1, 0, true, 1), new("c", 1, 1, 0, true, 0)] };
Check(Preview(explicitShares).Lines[1].AllocatedOrderDiscount == 1, "explicit complete allocation overrides proportional allocation");
Check(Preview(explicitShares with { Lines = [explicitShares.Lines[0] with { ExplicitOrderDiscount = null }, explicitShares.Lines[1], explicitShares.Lines[2]] })
    .Status == FinancialPreviewStatus.AwaitingEvidence, "partial explicit allocation is not completed by guessing");
Check(Has(explicitShares with { SellerOrderDiscount = 2 }, "AllocationTotalMismatch"), "explicit allocation sum must match order discount");
Check(Has(explicitShares with { Lines = [explicitShares.Lines[0], explicitShares.Lines[1] with { EligibleForOrderDiscount = false }, explicitShares.Lines[2]] },
    "InvalidExplicitAllocation"), "ineligible line cannot receive explicit discount");
Check(Has(allocation with { SellerOrderDiscount = 4 }, "DiscountExceedsEligibleTotal"), "order discount cannot exceed capacity");
var free = allocation with { Lines = [new("free", 1, 0, 0, true)], SellerOrderDiscount = 0, BuyerPayment = 0, ReportedInvoiceTotal = 0 };
Check(Preview(free).Status == FinancialPreviewStatus.Validated && Has(free with { SellerOrderDiscount = 1 }, "DiscountExceedsEligibleTotal"),
    "zero-weight free order works only with zero discount");
var fractional = free with { Lines = [new("fractional", .5m, 1, 0, true)], BuyerPayment = 1, ReportedInvoiceTotal = 1 };
Check(Preview(fractional).Gross == 1, "line midpoint rounds away from zero once");
Check(Has(sample with { Lines = [new("huge", 1_000_000, 999_999_999_999_999_999m, 0, true)] }, "FinancialAmountOverflow"),
    "out-of-range aggregate returns review not an exception");
var large = allocation with { Lines = [new("a", 1, 400_000_000_000_000_000m, 0, true), new("b", 1, 400_000_000_000_000_000m, 0, true)],
    SellerOrderDiscount = 400_000_000_000_000_001m, BuyerPayment = 399_999_999_999_999_999m, ReportedInvoiceTotal = 399_999_999_999_999_999m };
Check(Preview(large).Status == FinancialPreviewStatus.Validated && Preview(large).Lines.Sum(x => x.AllocatedOrderDiscount) == large.SellerOrderDiscount,
    "large allocation uses exact integer math beyond decimal product range");
var previous = CultureInfo.CurrentCulture;
try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fa-IR"); Check(JsonSerializer.Serialize(Preview(allocation)) == JsonSerializer.Serialize(allocated), "culture does not affect money or tie breaks"); }
finally { CultureInfo.CurrentCulture = previous; }
var seed = new Random(2026);
for (var n = 0; n < 200; n++)
{
    var lines = Enumerable.Range(0, seed.Next(1, 20)).Select(i => new FinancialPreviewLine(i.ToString("D3"), 1, seed.Next(1, 10000), 0, true)).ToArray();
    var total = lines.Sum(x => x.UnitPrice!.Value); var discount = seed.Next(0, (int)total + 1);
    var preview = Preview(allocation with { Lines = lines, SellerOrderDiscount = discount, BuyerPayment = total - discount, ReportedInvoiceTotal = total - discount });
    if (preview.Status != FinancialPreviewStatus.Validated || preview.Lines.Sum(x => x.AllocatedOrderDiscount) != discount
        || preview.Lines.Any(x => x.Net < 0) || preview.NetGoods != total - discount) throw new Exception("allocation invariant");
}
Check(true, "200 seeded allocation cases conserve money and nonnegative lines");
var parallel = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => Task.Run(() => JsonSerializer.Serialize(Preview(sample)))));
Check(parallel.All(x => x == JsonSerializer.Serialize(result)), "singleton calculator is deterministic and safe for concurrent previews");
Console.WriteLine($"{checks} financial checks passed; no SQL or external calls.");
