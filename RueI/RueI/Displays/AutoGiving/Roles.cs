namespace RueI.Extensions;

using PlayerRoles;

/// <summary>
/// Provides a means for describing multiple <see cref="RoleTypeId"/>s.
/// </summary>
public readonly struct Roles
{
    private readonly RoleTypeId[] roles;

    /// <summary>
    /// Initializes a new instance of the <see cref="Roles"/> struct.
    /// </summary>
    /// <param name="enumerableRoles">The roles to use.</param>
    public Roles(IEnumerable<RoleTypeId> enumerableRoles)
    {
        roles = enumerableRoles.Distinct().ToArray();
    }

    /// <summary>
    /// Combines two <see cref="Roles"/>.
    /// </summary>
    /// <param name="left">The first <see cref="Roles"/>.</param>
    /// <param name="right">The second <see cref="Roles"/>.</param>
    /// <returns>A combined <see cref="Roles"/>.</returns>
    public static Roles operator |(Roles left, Roles right) => new(left.roles.Union(right.roles));

    /// <summary>
    /// Determines whether or not this contains a certain <see cref="RoleTypeId"/>.
    /// </summary>
    /// <param name="roleId">The <see cref="RoleTypeId"/> to check.</param>
    /// <returns>A value indicating whether or not this has a certain <see cref="RoleTypeId"/>.</returns>
    public bool Has(RoleTypeId roleId) => roles.Contains(roleId);
}