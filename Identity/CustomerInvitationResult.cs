namespace Pistachio.Api.Identity;

public enum CustomerInvitationResult
{
    /// <summary>Provisioning is switched off, or the call failed.</summary>
    NotInvited,

    /// <summary>An account already existed for this address.</summary>
    AlreadyRegistered,

    /// <summary>An account was created and an invitation email was sent.</summary>
    Invited
}
