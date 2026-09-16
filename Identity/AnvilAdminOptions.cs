namespace Pistachio.Api.Identity;

public sealed class AnvilAdminOptions
{
    /// <summary>Disables provisioning without removing the configuration.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Base URL of the identity provider, without the realm path.</summary>
    public string BaseUrl { get; set; } = string.Empty;

    public string Realm { get; set; } = string.Empty;

    /// <summary>Confidential client with a service account allowed to manage users.</summary>
    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>Realm role granted to a customer account created by the application.</summary>
    public string DefaultRole { get; set; } = "pistachio-customer";

    /// <summary>Client the invitation link returns to once the password is set.</summary>
    public string InviteClientId { get; set; } = string.Empty;

    public string InviteRedirectUri { get; set; } = string.Empty;

    public int InviteLifespanSeconds { get; set; } = 86400;
}
