# Basalam webhook delivery contract

Verified 2026-09-26 against the [official webhook guide](https://developers.basalam.com/docs/services/webhook)
and [Python SDK example](https://developers.basalam.com/docs/sdk/python/webhook).
The old `/services/webhook` documentation URL returns 404; the current path includes `/docs`.

## Registration and routing

`BasalamWebhookEvents` is the shared event-type catalog used by registration and
Ingress. The official event IDs are **types**, not unique notification IDs:

| ID | Name | OAuth permission | Queue item |
| --- | --- | --- | --- |
| 1 | CHAT_RECEIVED_MESSAGE | customer.chat.read | Chat |
| 2 | ORDER_ITEM_CHANGES | customer.order.read | Purchase |
| 3 | VENDOR_ORDER_ITEM_CHANGES | vendor.parcel.read | Sale |
| 4 | CHAT_SEND_MESSAGE | customer.chat.read | Chat |
| 5 | VENDOR_NEW_ORDER | vendor.parcel.read | Sale |
| 6 | NEW_ORDER | customer.order.read | Purchase |
| 7 | VENDOR_PARCEL_CHANGES | vendor.parcel.read | Sale |
| 8 | PRODUCT_CREATE_CHANGES | vendor.product.read | Product |
| 9 | REVIEW_CREATE_CHANGES | vendor.parcel.read | Review |

Registration uses the documented `https://webhook.basalam.com/v1/webhooks`,
`event_ids`, `register_me=true`, and a configurable Authorization header. It uses
the selected connection's grant, not an application client secret as a booth grant.
Subscription is not one of these nine events; its source remains a separate task.
Requesting permissions in appsettings does not prove an existing grant has them.

## Explicit ingress contract and unverified provider metadata

The existing endpoint is
`POST /api/integrations/v1/webhooks/basalam/{vendorId}`.
It authenticates the configured per-connection Bearer webhook secret. This is
not `hyper-hmac-v1`, which belongs to the internal Hyperyek protocol.

Our explicit transport metadata is `X-Event-Id` and `X-Event-Type`; string fields
`event_id`/`eventId` and `event`/`event_type`/`eventType` are accepted as normalized
body alternatives. Header values take precedence. Basalam's root `id` and `name`
are **not** substitutes: they may identify/name a product or message. Numeric
event-type IDs are not converted into delivery IDs. An absent unique delivery
identity is rejected; neither random IDs nor a body hash are invented.
Internal non-Basalam legacy `id`/`name` fallbacks remain available.

**The public documents inspected do not establish that Basalam sends those
metadata headers or normalized body alternatives.** They document event
`sample_data`, not enough to prove the complete delivery envelope and retry ID.
Unauthenticated reads of both official event-list endpoints returned HTTP 401.
Before live acceptance, use an authorized real grant to obtain event samples and
redacted webhook delivery logs; verify the type discriminator, stable retry ID,
and successive updates to the same entity. Registration alone is not acceptance.

## Payload support and remaining limits

- Product notifications resolve the trusted mapping and re-read the provider
  catalog; webhook titles/prices do not directly become accounting values.
- Text-only `CHAT_RECEIVED_MESSAGE`/`CHAT_SEND_MESSAGE` accepts positive numeric
  `id`, `chat_id`, nested `message.text`, and maps direction explicitly. It can
  also unwrap the existing normalized `data`/`order`/`payload` object envelope.
  Message identity stays separate from the delivery identity. Files, nonempty
  links and entity content retain the raw Inbox and yield `NeedsAttention` with
  `ChatRichContentOwnerContractRequired`; no attachment is silently discarded.
- The production engagement owner is still unregistered. Text normalization
  does not implement a chat/review domain: default processing remains actionable
  with `EngagementOwnerApiNotRegistered`.
- Parcel changes now use authoritative provider detail reads and scoped invoice
  bindings; preparation/posting use durable commands with GET-only recovery.
  See [FLOW-C](PARCEL_LIFECYCLE.md) for exact supported states and remaining live gates.
- Review and thin order/item payload hydration still need authoritative
  samples, ownership checks and scenario-specific mapping. Routing their event
  names to a queue does not mean those business scenarios are complete.

## Regression evidence

`tools/SynchronizationFlowChecks` links the production controller, ingress,
registration, dispatcher and queue. Tests use disposable SQL, intercepted provider
HTTP and an in-memory engagement owner. They cover invalid JSON/provider, identity
separation, explicit metadata precedence, official chat **sample shape** (synthetic
positive IDs), direction, duplicates, invalid identities, rich-content deferral,
and routing every registered event. They are neither provider delivery captures,
auth-middleware tests nor real accounting/provider acceptance.
