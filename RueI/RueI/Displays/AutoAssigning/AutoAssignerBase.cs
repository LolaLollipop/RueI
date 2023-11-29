namespace RueI.Extensions;
using PlayerRoles;
using RueI.Displays;
using RueI.Elements;

/// <summary>
/// Represents a <see cref="ElemReference{T}"/> and its associated creator.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="elemRef">The <see cref="ElemReference{T}"/> to use.</param>
/// <param name="creator">A <see cref="Func{TResult}"/> that creates the element. if it does not exist.</param>
public record ElemRefResolver<T>(ElemReference<T> elemRef, Func<T> creator)
    where T : IElement
{
    /// <summary>
    /// Gets an instance of <typeparamref name="T"/> using the <see cref="ElemReference{T}"/>, or creates it.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to use.</param>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    public T GetFor(DisplayCore core) => core.GetElementOrNew(elemRef, creator);
}

/// <summary>
/// Represents the base class for all automatic element givers.
/// </summary>
public abstract class AutoElementBase
{
    private static readonly Dictionary<RoleTypeId, List<AutoElementBase>> AutoElements = new();

    static AutoElementBase()
    {
        PlayerRoleManager.OnRoleChanged += OnRoleChanged;
    }

    /// <summary>
    /// Gives this <see cref="AutoElement{T}"/> to a <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The core to give this to.</param>
    protected abstract void GiveTo(DisplayCore core);

    private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
    {
        if (prevRole.RoleTypeId != newRole.RoleTypeId && AutoElements.TryGetValue(newRole.RoleTypeId, out List<AutoElementBase> list))
        {
            DisplayCore core = DisplayCore.Get(hub);

            foreach (AutoElementBase autoElement in list)
            {
                autoElement.GiveTo(core);
            }
        }
    }
}

/// <summary>
/// Manages and automatically assigns elements to <see cref="ReferenceHub"/>s meeting a criteria.
/// </summary>
/// <typeparam name="T">The type of the element to give.</typeparam>
public class AutoElement<T> : AutoElementBase
    where T : IElement
{
    private readonly T? element;

    private readonly Func<DisplayCore, T>? creator;
    private readonly ElemReference<T>? reference;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoElement{T}"/> class.
    /// </summary>
    /// <param name="reference">The <see cref="ElemReference{T}"/> to use.</param>
    /// <param name="creator">A <see cref="Func{T, TResult}"/> that creates the elements.</param>
    public AutoElement(ElemReference<T> reference, Func<DisplayCore, T> creator)
    {
        this.reference = reference;
        this.creator = creator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoElement{T}"/> class.
    /// </summary>
    /// <param name="element">The <typeparamref name="T"/> to automatically give.</param>
    public AutoElement(T element)
    {
        this.element = element;
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
            core.AddAsReference(reference!.Value, creator!(core));
        }
    }
}