# Pistachio API

ASP.NET Core Web API for [Pistachio](https://github.com/fernandosilvapinto/pistachio),
a platform for selling and scheduling services. It owns the catalog, the bookings,
their fulfilment and the payment records.

## What it does

**Catalog.** Services with description, price, availability and a featured flag.
Listing is open, so a storefront can render it without a session; changing it
requires permission.

**Bookings.** A customer books a listed service for a date. Staff list every
booking, customers list only their own. A booking moves through pending,
confirmed, completed and cancelled.

**Fulfilment.** A booking can be assigned to the staff member who will deliver
the service.

**Guest booking.** A visitor books without an account. The API records the
booking, creates an account for them in the identity provider, and the provider
emails an invitation to set a password. The next visit is an ordinary sign-in.

**Payments.** Amounts tied to a booking and a service, with their own status.

**People.** A local directory of everyone the domain refers to, kept in step with
the identity provider automatically.

**Email.** Booking confirmations are sent over SMTP.

## Endpoints

| Method | Route | Requires |
|---|---|---|
| `GET` | `/api/services` | — |
| `POST` `PUT` `DELETE` | `/api/services` | `services:write` |
| `GET` | `/api/schedulings/mine` | `scheduling:read` |
| `GET` | `/api/schedulings` | `scheduling:read` |
| `POST` | `/api/schedulings/guest` | — |
| `POST` `PUT` | `/api/schedulings` | `scheduling:write` |
| `PATCH` | `/api/schedulings/{id}/status` | `scheduling:status` |
| `PATCH` | `/api/schedulings/{id}/mechanic` | `scheduling:assign` |
| `DELETE` | `/api/schedulings/{id}` | `scheduling:delete` |
| `GET` | `/api/payments` | `payments:read` |
| `POST` `PUT` `DELETE` | `/api/payments` | `payments:write` |
| `GET` | `/api/users` | `users:read` |
| `GET` | `/api/auth/me` | authenticated |

## Access control

The API does not authenticate anyone and does not issue tokens. It validates
access tokens issued by an external OpenID Connect provider, using signing keys
fetched from that provider's JWKS endpoint.

Endpoints are guarded by permissions in `resource:action` form, which arrive
inside the token. Roles never appear in application code, so an operator can
define whatever job titles their organization uses without the API changing.

Whether a person may act on a specific record — their own booking, for instance —
is domain logic and lives here. The provider says who the caller is and what
class of operation they may perform.

## Stack

| Layer | Technology |
|---|---|
| Runtime | ASP.NET Core, C# |
| Persistence | Entity Framework Core, PostgreSQL |
| Documentation | Swagger / OpenAPI |
| Packaging | Docker |

## Running

Run it through the [pistachio-infrastructure](https://github.com/fernandosilvapinto/pistachio-infrastructure)
Compose environment, which provides PostgreSQL and wires the external
dependencies. Configuration comes from environment variables:

| Variable | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL |
| `Keeper__Authority` | Realm URL of the identity provider |
| `Keeper__Audience` | This API's audience, validated on every token |
| `Keeper__Admin__*` | Service account used to create guest accounts |
| `Smtp__*` | Outbound mail |
| `ClientUrl` | Public address of the customer application |

Migrations run at startup, and sample data is seeded on an empty database.

## Layout

```
Controllers/     catalog, bookings, payments, people
Models/          entities
DTOs/            request and response contracts
Data/            DbContext, migrations, sample data
Services/        email delivery
Authorization/   permission policies
Identity/        token-to-person mapping and guest account creation
```
