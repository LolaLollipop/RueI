namespace RueI
{
    using MEC;
    using RueI.Interfaces;

    /// <summary>
    /// Represents a <see cref="Display"/> that hides elements based on an active screen.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    public class ScreenDisplay<T> : DisplayBase
        where T : Enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenDisplay{T}"/> class.
        /// </summary>
        /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
        /// <param name="defaultScreen">The default <see cref="T"/> to use as a screen.</param>
        public ScreenDisplay(ReferenceHub hub, T defaultScreen)
            : base(hub)
        {
            CurrentScreen = defaultScreen;
        }

        /// <summary>
        /// Gets a list of all <see cref="IScreenElement{T}"/>s of the display.
        /// </summary>
        public List<IScreenElement<T>> Elements { get; } = new();

        /// <summary>
        /// Gets or sets the current screen that the display is on.
        /// </summary>
        /// <remarks>Updating this does not automatically update the display.</remarks>
        public T CurrentScreen { get; set; }

        /// <inheritdoc/>
        public override IEnumerable<IElement> GetAllElements() => Elements;
    }

    /// <summary>
    /// Represents a display attached to a <see cref="DisplayCoordinator"/>.
    /// </summary>
    /// <include file='docs.xml' path='docs/members[@name="display"]/Display/*'/>
    public class Display : DisplayBase, IElementContainer
    {
        /// <summary>
        /// Gets the ratelimit used for displaying hints.
        /// </summary>
        public const float HINTRATELIMIT = 0.55f;

        private CoroutineHandle? rateLimitTask;
        private bool rateLimitActive = false;
        private bool shouldUpdate = false;

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
        /// <param name="coordinator">The <see cref="DisplayCoordinator"/> to assign the display to.</param>
        public Display(DisplayCoordinator coordinator)
            : base(coordinator)
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Display"/> class.
        /// </summary>
        ~Display()
        {
            if (rateLimitTask is CoroutineHandle ch)
            {
                Timing.KillCoroutines(ch);
            }
        }

        /// <summary>
        /// Gets the elements of this display.
        /// </summary>
        public List<IElement> Elements { get; } = new();

        public void Update() => DisplayCoordinator.Get(ReferenceHub).Update();

        /// <inheritdoc/>
        public override IEnumerable<IElement> GetAllElements() => Elements.Where(x => x.Enabled);
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
            Coordinator = DisplayCoordinator.Get(hub);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayBase"/> class.
        /// </summary>
        /// <param name="coordinator">The <see cref="DisplayCoordinator"/> to assign the display to.</param>
        public DisplayBase(DisplayCoordinator coordinator)
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
        /// Gets the <see cref="DisplayCoordinator"/> that this display is attached to.
        /// </summary>
        public DisplayCoordinator Coordinator { get; }

        /// <summary>
        /// Gets all of the elements of this display.
        /// </summary>
        /// <returns>The <see cref="IEnumerator{IElement}"/> of elements.</returns>
        public abstract IEnumerable<IElement> GetAllElements();

        /// <summary>
        /// Deletes this display, removing it from the player's coordinator.
        /// </summary>
        public void Delete()
        {
            Coordinator.RemoveDisplay(this);
            IsActive = false;
        }
    }
}
