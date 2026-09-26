# Bidirectional Hyperyek / Basalam acceptance

## SDK transport regression checks

Run `dotnet run --project tools/BasalamTransportChecks` from the Backend Git root.
The checks exercise the production SDK with an in-memory HTTP handler: loading,
refreshing and clearing a connection token must affect child services, distinct
clients must not share booth credentials, and root/section/explicit configuration
must all reach the SDK. Production relative routes resolve to HTTPS and stock zero
is sent as an explicit absolute PATCH. No real provider or business database is
called by this tool.

`BasalamClient.SetToken` now forwards to the shared transport used by its catalog,
product and other services. `AddBasalamSdk` accepts either root configuration or
the already-selected `Basalam` section and preserves an explicitly supplied
`BasalamConfig` instance. Public method signatures and MCP contracts are unchanged.

## Real-provider prerequisites (not replaced by fixture tests)

The selected integration database must contain the real shop/tenant connection,
an OAuth grant obtained through the panel's existing Basalam login, and explicit
product/variant mappings. Application ClientId/ClientSecret alone are not a booth
grant. Never paste those credentials into chat or task logs.

The configured OAuth scope list must allow both reading products and modifying
them (`vendor.product.read`, `vendor.product.write`); request only the additional
permissions needed by the selected scenario. These names are defined by the
[official Basalam SDK](https://github.com/basalam/python-sdk/blob/main/src/basalam_sdk/auth.py).
Changing requested scopes does not upgrade an existing grant: the booth owner
must authorize the updated request. Confirm the granted scopes before publishing.

Select an explicitly authorized test product and expected absolute inventory
before any live write. Do not enable broad capture/workers, alter arbitrary
prices/stock, or create customer orders simply to prove connectivity. Record
separately the SQL/HTTP fixture results and actual Basalam/accounting outcomes.
