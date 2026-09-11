# Identity registration

Registers this application with the Keeper identity provider: the resource
server and its permissions, the two front-end clients, and the business roles
that aggregate permissions.

```
./register.sh
```

The script is idempotent — running it again updates origins and leaves existing
objects untouched. It expects Keeper to be running and reachable, and locates it
through `KEEPER_HOME`, defaulting to a `keeper` directory alongside the
application repositories.

## Permissions

APIs authorize on permissions, never on business roles, so a new role can be
introduced without changing application code.

| Permission | Covers |
|---|---|
| `services:read` | Reserved for administrative listings; the public catalog is anonymous |
| `services:write` | Creating, updating and removing services |
| `scheduling:read` | Listing bookings, own or all |
| `scheduling:write` | Creating and updating bookings |
| `scheduling:status` | Changing a booking's status |
| `scheduling:assign` | Assigning the staff member who will deliver the service |
| `scheduling:delete` | Removing a booking |
| `payments:read` | Reading payments |
| `payments:write` | Creating, updating and removing payments |
| `users:read` | Listing the local reference rows for people |
| `users:write` | Reserved; creating and disabling people happens in the provider |

The public service catalog and the guest booking endpoint stay anonymous and
require no permission.

Roles are not managed here. They live in the identity provider, which is their
single source of truth, and reach the application through the access token.

## Roles

| Role | Permissions |
|---|---|
| `pistachio-customer` | `scheduling:read`, `scheduling:write` |
| `pistachio-staff` | plus `services:read`, `scheduling:status` |
| `pistachio-manager` | plus `services:write`, `scheduling:assign`, `scheduling:delete`, `payments:*`, `users:read` |
| `pistachio-admin` | plus `users:write` |

## Service account

The guest booking flow creates a customer account in the provider on the
visitor's behalf, so that signing up is one click away. That needs a
confidential client with a service account:

| Client | Realm management roles | Used for |
|---|---|---|
| `pistachio-provisioning` | `manage-users`, `view-realm` | Creating a customer account and sending the invitation email |

`register.sh` creates it and prints its secret. Pass `PISTACHIO_PROVISIONING_SECRET`
to make the value reproducible across environments, and keep it out of source
control.

This client can create users in the realm. It is deliberately separate from the
resource server, holds only the two roles it needs, and never receives a
password: the provider collects that itself through the invitation link.
