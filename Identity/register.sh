#!/usr/bin/env bash
set -euo pipefail

KEEPER_HOME=${KEEPER_HOME:-"$(cd "$(dirname "$0")/../../../keeper" && pwd)"}

API=pistachio-api
ADMIN_ORIGIN=${PISTACHIO_ADMIN_ORIGIN:-http://localhost:5173}
CLIENT_ORIGIN=${PISTACHIO_CLIENT_ORIGIN:-http://localhost:5174}

PERMISSIONS="services:read,services:write,\
scheduling:read,scheduling:write,scheduling:status,scheduling:assign,scheduling:delete,\
payments:read,payments:write,\
users:read,users:write"

"$KEEPER_HOME/register-api.sh" "$API" "$PERMISSIONS"

"$KEEPER_HOME/register-spa.sh" pistachio-admin  "$ADMIN_ORIGIN"  "$API"
"$KEEPER_HOME/register-spa.sh" pistachio-client "$CLIENT_ORIGIN" "$API"

"$KEEPER_HOME/register-role.sh" pistachio-customer \
"$API:scheduling:read,$API:scheduling:write"

"$KEEPER_HOME/register-role.sh" pistachio-staff \
"$API:services:read,$API:scheduling:read,$API:scheduling:write,$API:scheduling:status"

"$KEEPER_HOME/register-role.sh" pistachio-manager \
"$API:services:read,$API:services:write,\
$API:scheduling:read,$API:scheduling:write,$API:scheduling:status,$API:scheduling:assign,$API:scheduling:delete,\
$API:payments:read,$API:payments:write,\
$API:users:read"

"$KEEPER_HOME/register-service-client.sh" pistachio-provisioning \
"manage-users,view-realm" "${PISTACHIO_PROVISIONING_SECRET:-}"

"$KEEPER_HOME/register-role.sh" pistachio-admin \
"$API:services:read,$API:services:write,\
$API:scheduling:read,$API:scheduling:write,$API:scheduling:status,$API:scheduling:assign,$API:scheduling:delete,\
$API:payments:read,$API:payments:write,\
$API:users:read,$API:users:write"

echo
echo "Pistachio registered in Keeper."
