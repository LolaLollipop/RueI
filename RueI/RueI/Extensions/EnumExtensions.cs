namespace RueI.Extensions;

using PlayerRoles;
using RueI.Displays;

/// <summary>
/// Provides extensions for working with RueI <see cref="Enum"/>s.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Quickly determines if a <see cref="Roles"/> has another <see cref="Roles"/>.
    /// </summary>
    /// <param name="first">The first <see cref="Roles"/>.</param>
    /// <param name="second">The other <see cref="Roles"/>.</param>
    /// <returns>A value indicating whether or not the first has all of the flags of the second.</returns>
    public static bool HasFlagFast(this Roles first, Roles second) => (first & second) == second;

    /// <summary>
    /// Quickly determines if a <see cref="Roles"/> has a <see cref="RoleTypeId"/>.
    /// </summary>
    /// <param name="first">The first <see cref="Roles"/>.</param>
    /// <param name="second">The other <see cref="Roles"/>.</param>
    /// <returns>A value indicating whether or not the first has the <see cref="RoleTypeId"/> of the second.</returns>
    public static bool HasFlagFast(this Roles first, RoleTypeId second)
    {
        int toInt = (int)second;
        if (toInt == -1)
        {
            return false;
        }

        Roles secondCasted = (Roles)(1 << (int)second);
        return (first & secondCasted) == secondCasted;
    }
}
