#!/usr/bin/env bash
set -euo pipefail

KEEPER_HOME=${KEEPER_HOME:-"$(cd "$(dirname "$0")/../../../keeper" && pwd)"}

WORKFORCE_REALM=${WORKFORCE_REALM:-workforce}
CUSTOMERS_REALM=${CUSTOMERS_REALM:-customers}

API=pistachio-api
ADMIN_ORIGIN=${PISTACHIO_ADMIN_ORIGIN:-http://localhost:5173}
CLIENT_ORIGIN=${PISTACHIO_CLIENT_ORIGIN:-http://localhost:5174}

# ---------------------------------------------------------------------------
# Workforce realm — the operator's own people.
# The full permission catalog lives here.
# ---------------------------------------------------------------------------

export KEEPER_REALM=$WORKFORCE_REALM
echo "### $KEEPER_REALM"

"$KEEPER_HOME/register-api.sh" "$API" "services:read,services:write,\
scheduling:read,scheduling:write,scheduling:status,scheduling:assign,scheduling:delete,\
payments:read,payments:write,\
users:read,users:write"

"$KEEPER_HOME/register-spa.sh" pistachio-admin "$ADMIN_ORIGIN" "$API"

"$KEEPER_HOME/register-role.sh" pistachio-staff \
"$API:services:read,$API:scheduling:read,$API:scheduling:write,$API:scheduling:status"

"$KEEPER_HOME/register-role.sh" pistachio-manager \
"$API:services:read,$API:services:write,\
$API:scheduling:read,$API:scheduling:write,$API:scheduling:status,$API:scheduling:assign,$API:scheduling:delete,\
$API:payments:read,$API:payments:write,\
$API:users:read"

"$KEEPER_HOME/register-role.sh" pistachio-admin \
"$API:services:read,$API:services:write,\
$API:scheduling:read,$API:scheduling:write,$API:scheduling:status,$API:scheduling:assign,$API:scheduling:delete,\
$API:payments:read,$API:payments:write,\
$API:users:read,$API:users:write"

# ---------------------------------------------------------------------------
# Customers realm — people who buy services.
# Only the permissions a customer can ever hold are declared here, so the rest
# cannot be granted by accident.
# ---------------------------------------------------------------------------

export KEEPER_REALM=$CUSTOMERS_REALM
echo
echo "### $KEEPER_REALM"

"$KEEPER_HOME/register-api.sh" "$API" "scheduling:read,scheduling:write"

"$KEEPER_HOME/register-spa.sh" pistachio-client "$CLIENT_ORIGIN" "$API"

"$KEEPER_HOME/register-role.sh" pistachio-customer \
"$API:scheduling:read,$API:scheduling:write" default

"$KEEPER_HOME/register-service-client.sh" pistachio-provisioning \
"manage-users,view-realm" "${PISTACHIO_PROVISIONING_SECRET:-}"

if [ -d "$(dirname "$0")/theme/pistachio" ]; then
  "$KEEPER_HOME/set-realm-theme.sh" "$CUSTOMERS_REALM" pistachio
fi

echo
echo "Pistachio registered."
echo "  workforce realm : $WORKFORCE_REALM  (pistachio-admin)"
echo "  customers realm : $CUSTOMERS_REALM  (pistachio-client)"
