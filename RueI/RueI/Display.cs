namespace RueI;

using RueI.Extensions;
using RueI.Interfaces;

/// <summary>
/// Represents a <see cref="IElementContainer"/> inside a <see cref="ScreenDisplay"/>.
/// </summary>
public class Screen : IElementContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    /// <param name="scrDisplay">The <see cref="ScreenDisplay"/> to add this to.</param>
    public Screen(ScreenDisplay scrDisplay)
    {
        scrDisplay.Screens.Add(this);
    }

    /// <summary>
    /// Gets the elements of this screen.
    /// </summary>
    public List<IElement> Elements { get; } = new();
}

/// <summary>
/// Represents a display attached to a <see cref="DisplayCore"/> with support for <see cref="Screen"/>s.
/// </summary>
public class ScreenDisplay : DisplayBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenDisplay"/> class.
    /// </summary>
    /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
    /// <param name="screen">The default <see cref="Screen"/> to use for this <see cref="ScreenDisplay"/>.</param>
    public ScreenDisplay(ReferenceHub hub, Screen screen)
        : base(hub)
    {
        CurrentScreen = screen;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenDisplay"/> class.
    /// </summary>
    /// <param name="coordinator">The <see cref="DisplayCore"/> to assign the display to.</param>
    /// <param name="screen">The default <see cref="Screen"/> to use for this <see cref="ScreenDisplay"/>.</param>
    public ScreenDisplay(DisplayCore coordinator, Screen screen)
        : base(coordinator)
    {
        CurrentScreen = screen;
    }

    /// <summary>
    /// Gets the current screen of this display.
    /// </summary>
    public Screen CurrentScreen { get; private set; }

    /// <summary>
    /// Gets all of the screens of this display.
    /// </summary>
    public List<Screen> Screens { get; } = new();

    /// <summary>
    /// Gets the elements of this display that will be displayed regardless of screen.
    /// </summary>
    public List<IElement> GlobalElements { get; } = new();

    /// <summary>
    /// Sets the <see cref="CurrentScreen"/> of this display.
    /// </summary>
    /// <param name="screen">The <see cref="Screen"/> to set the <see cref="CurrentScreen"/> to.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="screen"/> is not a <see cref="Screen"/> within <see cref="Screens"/>.</exception>
    public void SetScreen(Screen screen)
    {
        if (Screens.Contains(screen))
        {
            CurrentScreen = screen;
        }
        else
        {
            throw new ArgumentOutOfRangeException("screen", "Must be a screen within the ScreenDisplay");
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<IElement> GetAllElements()
    {
        foreach (IElement element in CurrentScreen.Elements.FilterDisabled())
        {
            yield return element;
        }

        foreach (IElement element in GlobalElements.FilterDisabled())
        {
            yield return element;
        }
    }
}

/// <summary>
/// Represents a display attached to a <see cref="DisplayCore"/>.
/// </summary>
/// <include file='docs.xml' path='docs/members[@name="display"]/Display/*'/>
public class Display : DisplayBase, IElementContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Display"/> class.
    /// </summary>
    /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
    public Display(ReferenceHub hub)
        : base(hub)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Display"/> class.
    /// </summary>
    /// <param name="coordinator">The <see cref="DisplayCore"/> to assign the display to.</param>
    public Display(DisplayCore coordinator)
        : base(coordinator)
    {
    }

    /// <summary>
    /// Gets the elements of this display.
    /// </summary>
    public List<IElement> Elements { get; } = new();

    /// <inheritdoc/>
    public override IEnumerable<IElement> GetAllElements() => Elements.FilterDisabled();
}

/// <summary>
/// Defines the base class for all displays.
/// </summary>
public abstract class DisplayBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayBase"/> class.
    /// </summary>
    /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
    public DisplayBase(ReferenceHub hub)
    {
        ReferenceHub = hub;
        Coordinator = DisplayCore.Get(hub);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayBase"/> class.
    /// </summary>
    /// <param name="coordinator">The <see cref="DisplayCore"/> to assign the display to.</param>
    public DisplayBase(DisplayCore coordinator)
    {
        Coordinator = coordinator;
        ReferenceHub = coordinator.Hub;
    }

    /// <summary>
    /// Gets a value indicating whether or not this display is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Gets the <see cref="ReferenceHub"/> that this display is assigned to.
    /// </summary>
    public ReferenceHub ReferenceHub { get; }

    /// <summary>
    /// Gets the <see cref="DisplayCore"/> that this display is attached to.
    /// </summary>
    public DisplayCore Coordinator { get; }

    /// <summary>
    /// Gets all of the elements of this display.
    /// </summary>
    /// <returns>The <see cref="IEnumerator{IElement}"/> of elements.</returns>
    public abstract IEnumerable<IElement> GetAllElements();

    /// <summary>
    /// Updates the parent <see cref="DisplayCore"/> of this <see cref="DisplayBase"/>.
    /// </summary>
    public void Update() => DisplayCore.Get(ReferenceHub).Update();

    /// <summary>
    /// Deletes this display, removing it from the player's coordinator.
    /// </summary>
    public void Delete()
    {
        Coordinator.RemoveDisplay(this);
        IsActive = false;
    }
}
