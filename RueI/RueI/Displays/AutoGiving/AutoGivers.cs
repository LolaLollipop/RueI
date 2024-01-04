namespace RueI.Extensions;

using PlayerRoles;

using RueI.Displays;
using RueI.Displays.Scheduling;
using RueI.Elements;

/// <summary>
/// Manages and automatically assigns elements to <see cref="DisplayCore"/> instances meeting a criteria.
/// </summary>
public class AutoElement
{
    private record PeriodicUpdate(TimeSpan time, int priority, JobToken token);

    private const int AUTOUPDATEPRIORITY = 5;

    private static readonly List<AutoElement> AutoGivers = new();

    private readonly Element? element;

    private readonly Func<DisplayCore, Element>? creator;
    private readonly IElemReference<Element> reference;

    private PeriodicUpdate? periodicUpdate;

    static AutoElement()
    {
        PlayerRoleManager.OnRoleChanged += OnRoleChanged;
    }

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

        AutoGivers.Add(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoElement"/> class.
    /// </summary>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="reference">The <see cref="IElemReference{T}"/> to use.</param>
    /// <param name="element">The element to automatically give.</param>
    private AutoElement(Roles roles, IElemReference<Element> reference, Element element)
    {
        this.element = element;
        this.reference = reference;
        Roles = roles;

        AutoGivers.Add(this);
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
        return new AutoElement(roles, DisplayCore.GetReference<T>(), element);
    }

    /// <summary>
    /// Creates a new <see cref="AutoElement"/> using a <see cref="IElemReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Element"/>.</typeparam>
    /// <param name="roles">The <see cref="Roles"/> to use for the <see cref="AutoElement"/>.</param>
    /// <param name="creator">A <see cref="Func{T, TResult}"/> that creates the elements.</param>
    /// <returns>A new <see cref="AutoElement"/>.</returns>
    public static AutoElement Create<T>(Roles roles, Func<DisplayCore, T> creator)
        where T : Element
    {
        return new AutoElement(roles, DisplayCore.GetReference<T>(), (core) => creator(core));
    }

    /// <summary>
    /// Disables this <see cref="AutoElement"/>.
    /// </summary>
    public virtual void Disable()
    {
        AutoGivers.Remove(this);
    }

    /// <summary>
    /// Schedules an update for all players with one of the <see cref="Roles"/> every <paramref name="span"/>.
    /// </summary>
    /// <param name="span">How often to schedule an update.</param>
    /// <param name="priority">The priority of the update.</param>
    /// <returns>A reference to this <see cref="AutoElement"/>.</returns>
    public AutoElement UpdateEvery(TimeSpan span, int priority = 35)
    {
        periodicUpdate = new(span, priority, new());
        return this;
    }

    /// <summary>
    /// Gives this <see cref="AutoElement"/> to a <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to give to.</param>
    protected virtual void GiveTo(DisplayCore core)
    {
        if (element != null)
        {
            core.AddAsReference(reference, element);
        }
        else
        {
            core.AddAsReference(reference, creator!(core));
        }

        if (periodicUpdate != null)
        {
            ScheduleUpdate(core, periodicUpdate);
        }
    }

    /// <summary>
    /// Removes this <see cref="AutoElement"/> from a <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="core">The <see cref="DisplayCore"/> to give to.</param>
    protected virtual void RemoveFrom(DisplayCore core)
    {
        core.RemoveReference(reference);

        if (periodicUpdate != null)
        {
            core.Scheduler.KillJob(periodicUpdate.token);
        }
    }

    private static void OnRoleChanged(ReferenceHub hub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
    {
        if (prevRole.RoleTypeId != newRole.RoleTypeId)
        {
            DisplayCore core = DisplayCore.Get(hub);

            foreach (AutoElement autoElement in AutoGivers)
            {
                if (autoElement.Roles.HasFlagFast(newRole.RoleTypeId))
                {
                    autoElement.GiveTo(core);
                }
                else
                {
                    autoElement.RemoveFrom(core);
                }
            }

            core.Update(AUTOUPDATEPRIORITY);
        }
    }

    private static void ScheduleUpdate(DisplayCore core, PeriodicUpdate update)
    {
        core.Scheduler.Schedule(update.time, () => ScheduleUpdate(core, update), update.token);
    }
}