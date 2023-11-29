namespace RueI.Displays;

using RueI.Elements;
using Hints;
using RueI.Displays.Scheduling;
using RueI.Extensions;

/// <summary>
/// Is responsible for managing all of the <see cref="DisplayBase"/>s for a <see cref="ReferenceHub"/>.
/// </summary>
public class DisplayCore
{
    private readonly List<DisplayBase> displays = new();
    private readonly Dictionary<ElemReference<IElement>, IElement> referencedElements = new();

    static DisplayCore()
    {
        RoundRestarting.RoundRestart.OnRestartTriggered += OnRestart;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayCore"/> class.
    /// </summary>
    /// <param name="hub">The hub to create the display for.</param>
    protected DisplayCore(ReferenceHub hub)
    {
        Hub = hub;

        if (hub != null)
        {
            DisplayCores.Add(hub, this);
        }

        Scheduler = new(this);
        AnonymousDisplay = new(this);
    }

    /// <summary>
    /// Gets the <see cref="Scheduling.Scheduler"/> for this <see cref="DisplayCore"/>.
    /// </summary>
    public Scheduler Scheduler { get; }

    /// <summary>
    /// Gets a dictionary containing the DisplayCores for each ReferenceHub.
    /// </summary>
    internal static Dictionary<ReferenceHub, DisplayCore> DisplayCores { get; } = new();

    /// <summary>
    /// Gets a display of anonymous <see cref="IElement"/>s added to this display.
    /// </summary>
    internal Display AnonymousDisplay { get; }

    /// <summary>
    /// Gets the <see cref="ReferenceHub"/> that this display is for.
    /// </summary>
    internal ReferenceHub Hub { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not updates will currently be ignored.
    /// </summary>
    internal bool IgnoreUpdate { get; set; } = false;

    /// <summary>
    /// Gets a DisplayCore from a <see cref="ReferenceHub"/>, or creates it if it doesn't exist.
    /// </summary>
    /// <param name="hub">The hub to get the display for.</param>
    /// <returns>The DisplayCore.</returns>
    public static DisplayCore Get(ReferenceHub hub)
    {
        if (DisplayCores.TryGetValue(hub, out DisplayCore value))
        {
            return value;
        }
        else
        {
            return new DisplayCore(hub);
        }
    }

    /// <summary>
    /// Updates this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="priority">The priority of the update - defaults to 100.</param>
    public void Update(int priority = 100)
    {
        if (IgnoreUpdate)
        {
            return;
        }

        Scheduler.Schedule(TimeSpan.Zero, () => { }, priority);
    }

    /// <summary>
    /// Gets an <see cref="IElement"/> as <typeparamref name="T"/> if the <see cref="ElemReference{T}"/> exists within this <see cref="DisplayCore"/>'s element references.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IElement"/> to get.</typeparam>
    /// <param name="reference">The <see cref="ElemReference{T}"/> to use.</param>
    /// <returns>The instance of <typeparamref name="T"/> if the <see cref="IElement"/> exists within the <see cref="DisplayCore"/>'s element references, otherwise null.</returns>
    public T? GetElement<T>(ElemReference<T> reference)
        where T : IElement
    {
        if (referencedElements.TryGetValue(reference, out IElement value) && value is T casted)
        {
            return casted;
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Gets an <see cref="IElement"/> as <typeparamref name="T"/>, or creates it.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IElement"/> to get.</typeparam>
    /// <param name="reference">The <see cref="ElemReference{T}"/> to use.</param>
    /// <param name="creator">A function that creates a new instance of <typeparamref name="T"/> if it does not exist.</param>
    /// <returns>The instance of <typeparamref name="T"/>.</returns>
    public T GetElementOrNew<T>(ElemReference<T> reference, Func<T> creator)
        where T : IElement
    {
        if (referencedElements.TryGetValue(reference, out IElement value) && value is T casted)
        {
            return casted;
        }
        else
        {
            T created = creator();
            referencedElements.Add(reference, created);
            return created;
        }
    }

    /// <summary>
    /// Adds an <see cref="IElement"/> as an <see cref="ElemReference{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IElement"/> to add.</typeparam>
    /// <param name="reference">The <see cref="ElemReference{T}"/> to use.</param>
    /// <param name="element">The <see cref="IElement"/> to add.</param>
    public void AddAsReference<T>(ElemReference<T> reference, T element)
        where T : IElement
    {
        referencedElements[reference] = element;
    }

    /// <summary>
    /// Sets the content of a <see cref="SetElement"/> <see cref="ElemReference{T}"/>, or creates it.
    /// </summary>
    /// <param name="reference">The <see cref="ElemReference{T}"/> to use.</param>
    /// <param name="content">The new content of the <see cref="SetElement"/>.</param>
    /// <param name="position">The position of the <see cref="SetElement"/> if it needs to be created.</param>
    public void SetElementOrNew(ElemReference<SetElement> reference, string content, float position)
    {
        if (referencedElements.TryGetValue(reference, out IElement value))
        {
            if (value is SetElement element)
            {
                element.Set(content);
            }
        }
        else
        {
            SetElement element = new(position, content);
            referencedElements.Add(reference, element);
        }
    }

    /// <summary>
    /// Updates this display, skipping all checks.
    /// </summary>
    internal void InternalUpdate()
    {
        string text = ElemCombiner.Combine(GetAllElements());
        Hub.connectionToClient.Send(new HintMessage(new TextHint(text, new HintParameter[] { new StringHintParameter(text) }, new HintEffect[] { HintEffectPresets.FadeIn(0, 0, 1) }, 99999)));
    }

    /// <summary>
    /// Adds a display to this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="display">The display to add.</param>
    internal void AddDisplay(DisplayBase display) => displays.Add(display);

    /// <summary>
    /// Removes a display from this <see cref="DisplayCore"/>.
    /// </summary>
    /// <param name="display">The display to remove.</param>
    internal void RemoveDisplay(DisplayBase display) => displays.Remove(display);

    /// <summary>
    /// Cleans up the dictionary after the server restarts.
    /// </summary>
    private static void OnRestart()
    {
        DisplayCores.Clear();
    }

    private IEnumerable<IElement> GetAllElements() => displays.SelectMany(x => x.GetAllElements())
                                                      .Concat(referencedElements.Values.FilterDisabled());
}