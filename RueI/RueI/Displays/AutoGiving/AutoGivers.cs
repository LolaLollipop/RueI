namespace RueI.Extensions;

using PlayerRoles;
using RueI.Displays;
using RueI.Elements;

/// <summary>
/// Represents a <see cref="IElemReference{T}"/> and its associated creator.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="elemRef">The <see cref="IElemReference{T}"/> to use.</param>
/// <param name="creator">A <see cref="Func{TResult}"/> that creates the element. if it does not exist.</param>
public record ElemRefResolver<T>(IElemReference<T> elemRef, Func<T> creator)
    where T : Element
{
    /// <summary>
    /// Gets an instance of <typeparamref name="T"/> using the <see cref="IElemReference{T}"/>, or creates it.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to use.</param>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    public T GetFor(DisplayCore core) => core.GetElementOrNew(elemRef, creator);
}

/// <summary>
/// Represents the base class for all automatic element givers.
/// </summary>
public abstract class AutoGiverBase
{
    private static readonly Dictionary<RoleTypeId, List<AutoGiverBase>> AutoElements = new();

    static AutoGiverBase()
    {
        PlayerRoleManager.OnRoleChanged += OnRoleChanged;
    }

    /// <summary>
    /// Gets a list of all active instances of <see cref="AutoGiverBase"/>s.
    /// </summary>
    protected static List<WeakReference<AutoGiverBase>> AutoGivers { get; } = new();

    /// <summary>
    /// Gives this <see cref="AutoElement"/> to a <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The core to give this to.</param>
    protected abstract void GiveTo(DisplayCore core);

    private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
    {
        if (prevRole.RoleTypeId != newRole.RoleTypeId && AutoElements.TryGetValue(newRole.RoleTypeId, out List<AutoGiverBase> list))
        {
            DisplayCore core = DisplayCore.Get(hub);

            foreach (AutoGiverBase autoElement in list)
            {
                autoElement.GiveTo(core);
            }
        }
    }
}

/// <summary>
/// Manages and automatically assigns elements to <see cref="ReferenceHub"/>s meeting a criteria.
/// </summary>
public class AutoElement : AutoGiverBase
{
    private readonly Element? element;

    private readonly Func<DisplayCore, Element>? creator;
    private readonly IElemReference<Element>? reference;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoElement"/> class.
    /// </summary>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <param name="creator">A <see cref="Func{T, TResult}"/> that creates the elements.</param>
    private AutoElement(Roles roles, IElemReference<Element> reference, Func<DisplayCore, Element> creator)
    {
        this.reference = reference;
        this.creator = creator;
        Roles = roles;

        AutoGivers.Add(new(this));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoElement"/> class.
    /// </summary>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="element">The <typeparamref name="T"/> to automatically give.</param>
    private AutoElement(Roles roles, Element element)
    {
        this.element = element;
        Roles = roles;

        AutoGivers.Add(new(this));
    }

    /// <summary>
    /// Gets or sets the roles that this <see cref="AutoElement"/> will give this element on.
    /// </summary>
    public Roles Roles { get; set; }

    /// <summary>
    /// Creates a new <see cref="AutoElement"/> of a shared <see cref="Element"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/>.</typeparam>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="element">The <paramref name="element"/> to use.</param>
    /// <returns>A new <see cref="AutoElement"/>.</returns>
    public static AutoElement Create<T>(Roles roles, T element)
        where T : Element
    {
        return new AutoElement(roles, element);
    }

    /// <summary>
    /// Creates a new <see cref="AutoElement"/> using a <see cref="ElemReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/>.</typeparam>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="reference">The <see cref="ElemReference{T}"/> to use.</param>
    /// <param name="creator">A <see cref="Func{T, TResult}"/> that creates the elements.</param>
    /// <returns>A new <see cref="AutoElement"/>.</returns>
    public static AutoElement Create<T>(Roles roles, IElemReference<T> reference, Func<DisplayCore, T> creator)
        where T : Element
    {
        return new AutoElement(roles, reference, (core) => creator(core));
    }

    /// <inheritdoc/>
    protected override void GiveTo(DisplayCore core)
    {
        if (element != null)
        {
            if (!core.AnonymousDisplay.Elements.Contains(element))
            {
                core.AnonymousDisplay.Elements.Add(element);
            }
        }
        else
        {
            core.AddAsReference(reference!, creator!(core));
        }
    }
}