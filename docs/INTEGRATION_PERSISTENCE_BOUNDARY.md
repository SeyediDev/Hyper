# Integration persistence boundary

Integration owns its persistence in the database configured by
`ConnectionStrings:IntegrationConnection` (normally `HyperyekIntegration`).
The Integration context is `HyperIntegrationContext`; it must not be replaced
by the Hyperyek accounting context.

The schema is provisioned by:

```text
docs/schema/ensure-integration-database.sql
tools/IntegrationSchemaProvisioner
```

The provisioner verifies these 14 tables:

`ExternalIntegrationConnections`, `ExternalOAuthTokens`,
`IntegrationCustomerMappings`, `IntegrationScenarioJobs`,
`ExternalProductMappings`, `ExternalOrderMappings`, `IntegrationSyncRuns`,
`IntegrationWebhookInbox`, `IntegrationOutbox`, `IntegrationMerchantAccess`,
`IntegrationAdminSimulations`, `IntegrationTokenRequests`,
`IntegrationEventAudits`, `InventoryReservationLogs`.

All generated Integration entity configurations use `ExcludeFromMigrations`.
The Hyperyek command model snapshot intentionally contains no Integration
tables. Integration schema changes are therefore applied by the dedicated
provisioner/script and never by the accounting migration pipeline.

Cross-domain data is represented by scalar identifiers and contracts. The
Integration API/worker do not own or migrate Hyperyek accounting tables.
