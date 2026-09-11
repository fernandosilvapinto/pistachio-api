# Login theme

Branding for the sign-in screen the customer application sends people to.

The identity provider owns authentication — the application never handles a
password — but a customer should not feel handed off to a different product
halfway through a purchase. This theme changes only presentation: colors,
typography, the card and the wordmark. No template is overridden, so upgrading
the provider does not break it.

## Installing

Copy or link the `pistachio` folder into the identity provider's themes
directory, then apply it to the customer realm:

```
./set-realm-theme.sh customers pistachio
```

`identity/register.sh` applies it automatically when the folder is present.

In development the provider runs with theme caching off, so CSS edits show on a
page refresh.

## Scope

The workforce realm keeps the provider's default appearance. Internal staff know
which system they are signing in to, and a neutral screen there is a feature:
it makes an unexpected branded login easier to spot.
