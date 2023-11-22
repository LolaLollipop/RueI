﻿namespace RueI;

using Hints;
using RueI.Displays;

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
        this.Hub = hub;

        if (hub != null)
        {
            DisplayCores.Add(hub, this);
        }

        Scheduler = new(this);
    }

    /// <summary>
    /// Gets the <see cref="Displays.Scheduler"/> for this <see cref="DisplayCore"/>.
    /// </summary>
    public Scheduler Scheduler { get; }

    /// <summary>
    /// Gets a dictionary containing the DisplayCores for each ReferenceHub.
    /// </summary>
    internal static Dictionary<ReferenceHub, DisplayCore> DisplayCores { get; } = new();

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
        referencedElements.Add(reference, element);
    }

    /// <summary>
    /// Updates this display, skipping all checks.
    /// </summary>
    internal void InternalUpdate()
    {
        string text = ElemCombiner.Combine(GetAllElements());
        Hub.connectionToClient.Send(new HintMessage(new TextHint(text, new HintParameter[] { new StringHintParameter(text) }, null, 99999)));
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
                                                      .Concat(referencedElements.Values.Where(x => x.Enabled));
}