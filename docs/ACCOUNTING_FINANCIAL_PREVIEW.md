# ACC-202: no-write financial preview

The proposed model in [the financial design](ACCOUNTING_FINANCIAL_DESIGN.fa.md)
now has an additive calculation-only implementation. It does not change existing
v1 invoice commands, database schema, stock gates, financial journals or provider
configuration. The new `Hyperyek.Accounting.Domain` references only the accounting
contracts project; it has no SQL, Core, Integration, Neo.Bpms or network dependency.
The existing accounting API registration wires the stateless calculator.

## API

`POST /api/hyperyek/v2/accounting/financial-preview` uses the accounting host's
existing Neo JWT service-client/scope policy. The request limit is 1 MiB, at most
1,000 unique lines. This is a trusted-service calculation endpoint, not merchant
authorization: scope is structurally checked, **not** resolved against live shop
ownership or provider data. Never treat the supplied scope, verified-unit flag or
a Validated result as proof of source authenticity or permission to post.

Example JSON (all money in source units):

```json
{
  "scope": { "shopId": 7, "tenantId": "tenant-a" },
  "connectionId": 10,
  "externalOrderId": "order-example",
  "billingGroupId": "merchant-parcel-group",
  "sourceVersion": 1,
  "policyVersion": "irr-preview-v1",
  "sourceUnit": 1,
  "sourceUnitVerified": true,
  "lines": [
    {
      "lineId": "one",
      "quantity": 1,
      "unitPrice": 10000000,
      "sellerDiscount": 1000000,
      "eligibleForOrderDiscount": true
    }
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
```

Source units: `1`=IRR, `2`=explicit Toman (multiply by ten once); `0` and other
values are unsupported. This does not assert which unit Basalam uses. Callers
must first establish their source unit. All response amounts, including Components
and line allocations, are normalized IRR. No conversion is applied to quantities.

The sample returns invoice=9,600,000, buyer payable=9,100,000 and expected
settlement=8,700,000, with `previewOnly=true`. Settlement is not profit or a bank
receipt. No journal suggestions, persisted snapshots or execution receipts are
created by this endpoint yet.

HTTP 200 means the calculation was evaluated, not that it was accepted for
posting. Inspect `status` and `issues`: `1`=Validated, `2`=AwaitingEvidence,
`3`=NeedsReview. Invalid authentication is 401, insufficient service authority 403;
malformed JSON/model binding is 400. Financial validation issues stay in the
typed result. Issues report codes and field paths, never credential/provider data.

## Exact supported profile

- One supplied merchant billing group, full customer payment, explicitly known
  funding split and taxes. There are no default tax rates, account codes or fee
  percentages. Partial/credit payment lifecycle is not inferred.
- Every money component is nullable. Missing or null is unknown; explicit zero
  is known zero. `reportedInvoiceTotal` is the one optional comparison value:
  omit it to skip that additional comparison, not to claim provider reconciliation.
- Invoice = net goods + merchant shipping + supplied invoice tax. Buyer payable
  = invoice - confirmed platform funding + directly collected third-party
  shipping. The third-party amount does not enter the seller invoice/settlement.
- Expected settlement = invoice - commission - commission tax - documented
  deductions + documented credits. It is withheld on any missing evidence or
  review issue. Deductions/credits must be disjoint totals from the upstream
  normalized snapshot; duplicate source-entry validation is not implemented here.
- Unknown commission can coexist with a computed invoice and buyer amount, but
  settlement is null. Unknown invoice tax prevents an invoice total. Computed
  partial totals are explanatory only. A negative settlement requires review.
- Amounts are nonnegative and below 10^18 IRR; final components must be integer
  IRR. Unit prices may have two IRR decimal places; quantity is positive, at most
  10^6 and at most three decimals. Line gross rounds once, AwayFromZero, to IRR.
  Aggregate overflow returns NeedsReview, never wrapping or silently clamping.
- Each line's direct seller discount is a **line total**, not per-unit. Overall
  seller discount is allocated over eligible net lines. Integer quotient and
  remainder arithmetic avoids decimal product overflow. Largest remainder wins,
  then ordinal LineId, independent of input order and machine culture.
- Optional `explicitOrderDiscount` is a complete-all-lines override: if any line
  supplies it, every line must supply it (zero for excluded lines). It must respect
  capacity and exactly sum to sellerOrderDiscount; partial shares are not guessed.
- Merchant freight income, third-party freight, actual carrier expense and
  withheld carrier costs are different concepts. Actual out-of-settlement carrier
  expense is not an input here. Do not deduct it from expected settlement again.
  Collect-on-delivery and agent-held third-party funds need a separate profile.

`irr-preview-v1` is an exact proposed software calculation profile, not a
production accounting/tax approval. Tax semantics, recognition policy, native
account mappings, multi-merchant allocations, refunds and posting remain outside
this implementation. Caller-supplied SourceVersion identifies an input; no
idempotency store, snapshot hash or live source verification is claimed.

## Checks

```powershell
dotnet build tools/AccountingFinancialChecks/AccountingFinancialChecks.csproj --artifacts-path .artifacts/financial-preview -m:1 -p:UseSharedCompilation=false -p:NuGetAudit=false
dotnet exec .artifacts/financial-preview/bin/AccountingFinancialChecks/debug/AccountingFinancialChecks.dll
```

The pure checks need no SQL/provider and cover the sample, zero/unknown, currency,
freight ownership, fees, invalid inputs, fractional quantity/rounding, explicit and
proportional allocations, large-number arithmetic, culture and concurrency,
including 200 seeded conservation cases. `AccountingSecurityChecks` additionally
runs the real preview controller with Neo JWT on ephemeral loopback Kestrel,
without registering any SQL or persistence implementation. It verifies authorized
calculations and 401/403/400 responses. Loopback HTTP is a test fixture only;
production TLS requirements remain unchanged. Builds with NuGetAudit=false do not
constitute vulnerability audits.

Verified on 2026-09-26: 40 financial assertions (including the 200 seeded cases)
and 38 authentication/HTTP assertions passed. Final builds of both check projects,
including Accounting.Api and Accounting.Host, had zero warnings/errors. Initial
compilation found a nullable lines guard, which was corrected; a stale contract
reference generated during an overlapping source edit required a targeted
non-incremental Contracts rebuild before the final successful host test. No
tests contacted SQL, the real IdP or Basalam. No full-solution build is claimed.

## Integration mapping checkpoint (ACC-201)

`IIntegrationFinancialPreviewPort` now exposes provider-neutral request/result
records in Integration.Domain. Only the HTTP adapter in Integration.Infrastructure
references Accounting.Contracts; Integration.Domain does not reference accounting
or reproduce its calculator. `AccountingFinancialPreviewClient` is registered by
`AddHyperyekAccountingClients` with the existing authenticated accounting service
client. No new credential, merchant token, database or Neo.Bpms dependency is added.

Usage from an Integration consumer (resolve the port through dependency injection):

```csharp
IntegrationFinancialPreviewResult result =
    await financialPreview.PreviewAsync(normalizedEvidence, cancellationToken);
// Display Status, Issues, Components and explanatory totals.
// Never turn Validated into an ApplyVendorOrder call or a posting permission.
```

The mapper preserves tenant/shop/connection/order/billing-group/version, every
nullable amount, explicit discount allocation, unit and verification flag. It
does not convert Toman, infer missing fees or taxes, load a webhook, authorize
shop ownership, fetch provider data, create customer mappings, or post a document.
The result remains preview-only. Production callers must supply normalized evidence
from a trusted source. The manual panel workflow below now calls this port with
explicitly synthetic inputs. The dispatcher does not call it, and the real
provider-to-preview-to-display workflow is **not** complete at this checkpoint.

Only HTTP 200 with a recognized preview response is mapped. The actual wire
`previewOnly` flag must be true; malformed/unknown status, policy or currency,
missing result collections, foreign/duplicate line IDs and incomplete Validated
results fail closed. Nonvalidated results must have matching issue severity and
no settlement. These checks validate the response contract, not the underlying
accounting arithmetic or the authenticity of the source data.

Responses are bounded to 1 MiB, and the HTTP timeout covers body reading as well
as headers. Errors expose stable codes, not raw response bodies or nested
transport exceptions. HTTP 408/429/5xx, network failures and timeouts are retryable;
caller cancellation propagates. The existing auth handler owns 401 token
invalidation; neither it nor this adapter blindly replays a POST. Retry scheduling
belongs to a future caller/durable workflow, not the financial calculator.

`AccountingSecurityChecks` includes real local authenticated-controller bridge
tests plus controlled transport/mapping/error fixtures. Run:

```powershell
dotnet build tools/AccountingSecurityChecks/AccountingSecurityChecks.csproj --artifacts-path .artifacts/platform-api-accounting -m:1 -p:UseSharedCompilation=false -p:NuGetAudit=false
dotnet exec .artifacts/platform-api-accounting/bin/AccountingSecurityChecks/debug/AccountingSecurityChecks.dll
```

Checkpoint verified on 2026-09-26: **99** authentication/HTTP/mapping assertions
passed (61 added since the 38-check calculator endpoint stage), including preview
through the registered service client, 401 invalidation without replay and token
refresh on the next explicit attempt. The existing **40** pure financial checks
were rerun successfully. The initial check-project build included dependencies;
the final test-only rebuild reused those built references. Both had zero warnings
and errors. This is targeted verification, not a full solution or live-provider test.

## Manual panel preview (ACC-201.PREVIEW-UI)

Open `/AccountingFinancialPreview/Index` on the AdminPanel host while logged in
as an administrator. This is a separate page; existing simulator/dashboard views
and the shared navigation are not changed. Use its link to `/MerchantSimulation`
to select a shop, then return to the preview URL. An enabled Basalam connection
must belong to that exact shop and tenant. This page does not request merchant
tokens or fetch anything from Basalam.

The panel invokes `IIntegrationFinancialPreviewPort`; it does not reference the
accounting calculator or read/write accounting tables. The dedicated accounting
service HTTPS/token settings described above must be configured on the panel
host. The operator's merchant/admin token is not reused for accounting access.

The editable JSON contains financial draft fields only. Shop, tenant, connection,
policy and source-verification fields cannot be inserted into it. Shop/tenant
come from the live server simulation, connection from its scoped enabled list,
and policy is fixed to `irr-preview-v1`. The unit acknowledgement is deliberately
unchecked on initial display. It means the operator checked the unit for this
**manual simulation**, not that Basalam data has been authenticated. Output is
always labelled synthetic and preview-only, with no apply/submit-to-accounting
button. Unknown totals display as unknown, not zero.

The page uses existing admin authentication, an explicit admin gate, antiforgery,
no-store response caching and a 256 KiB request limit. Editable JSON is limited
to 100,000 characters, depth 8 and 1,000 lines; duplicate or unrecognized fields
are rejected. Both the posted protected context ticket and current simulation
cookie must identify the same administrator-owned active simulation. The
simulation is rechecked after the financial call before displaying its result.
The page does not cache drafts/results in a database or queue a scenario.

Manual acceptance steps:

1. Sign in as an admin, select a shop in the existing simulator, open the preview
   URL, and select its active Basalam connection. No selection or connection
   should show an actionable empty state.
2. Leave the sample JSON unchanged, acknowledge its IRR unit, and calculate.
   Expect invoice 9,600,000, buyer 9,100,000 and settlement 8,700,000 IRR.
3. Set `platformCommission` to `null`: expect AwaitingEvidence and unknown
   settlement, while the invoice remains calculable.
4. Restore the sample and change `buyerPayment` to `1`: expect NeedsReview and
   a payment issue, not a successful posting. Uncheck unit acknowledgement:
   expect missing unit evidence and no calculated totals.
5. Keep an old form open, change shops in a second tab, then submit the old form:
   expect HTTP 409 without an accounting call. Repeat after ending/expiring the
   simulation. A different admin's ticket or another shop's connection must fail.
6. Insert `shopId`/`tenantId` or a duplicate monetary field into JSON: expect
   HTTP 400. A POST without antiforgery must be rejected by MVC. A non-admin must
   not gain access. Verify that error output does not expose tokens/settings.
7. Confirm no invoice, stock, token request, inbox or outbox is created. The page
   should only read simulation/connection context and call the no-write preview.

Focused workflow checks (fake ports; no live SQL/identity/provider):

```powershell
dotnet build tools/FinancialPreviewPanelChecks/FinancialPreviewPanelChecks.csproj --artifacts-path .artifacts/panel-financial-checks -m:1 -p:UseSharedCompilation=false -p:NuGetAudit=false
dotnet exec .artifacts/panel-financial-checks/bin/FinancialPreviewPanelChecks/debug/FinancialPreviewPanelChecks.dll
```

These workflow checks are not browser, real MVC authorization/antiforgery or live
accounting acceptance. Compile the actual AdminPanel/Razor as a separate targeted
build and perform the manual steps against the configured host before release.

Verified on 2026-09-26: 45 workflow assertions passed, including safe handling of
an unavailable accounting-backed shop lookup before and after form submission.
The unchanged 99-check accounting HTTP/authentication fixture binary was rerun
successfully. The initial AdminPanel build included dependencies and Razor and
succeeded with zero warnings/errors. The final reference-reusing build also
passed with zero warnings/errors, including the final workflow/controller/view changes:

```powershell
dotnet build src/AdminPanel/Hyper.AdminPanel.Web/Hyper.AdminPanel.Web.csproj --no-restore --artifacts-path .artifacts/platform-api-accounting -m:1 -p:BuildProjectReferences=false -p:UseSharedCompilation=false -p:NuGetAudit=false
```

Do **not** add `--no-incremental` or invoke Rebuild for this narrow check: this
repository hooks Clean/Rebuild to npm installation and shared microfrontend
builds. An intermediate attempt hit that hook and failed with npm EPERM on the
shared EditableGrid package-lock; no elevated npm attempt was made. This is not
a successful microfrontend build. Shared package.json/package-lock had no tracked
changes afterward. Asset copies generated by normal panel builds are excluded
from this feature's commit. Browser and live-host acceptance remain pending.

Next: validate real provider financial snapshots and connect the authoritative caller,
obtain accounting policy/account mapping acceptance, then implement versioned
posting and reconciliation independently. WorkManagement owns task status.
