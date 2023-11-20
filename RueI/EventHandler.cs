namespace RueI;

using PlayerRoles;

/// <summary>
/// Handles events for RueI.
/// </summary>
public static class EventHandler
{
#pragma warning disable SA1600 // Elements should be documented
    /// <summary>
    /// Called after the server restarts.
    /// </summary>
    public static void OnRestart()
    {
    }

    public static void OnPlayerRemoved(ReferenceHub hub)
    {
    }

    public static void OnServerRoleSet(ReferenceHub hub, RoleTypeId type, RoleChangeReason reason)
    {
        if (reason == RoleChangeReason.Died)
        {

        }
        else if (reason == RoleChangeReason.Destroyed)
        {

        }
    }
}
#pragma warning restore SA1600 // Elements should be documented