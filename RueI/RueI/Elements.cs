namespace RueI
{
    using RueI.Delegates;
    using RueI.Interfaces;
    using RueI.Records;

    /// <summary>
    /// Represents an element that is shown when certain screens are active.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    public interface IScreenElement<T> : IElement
        where T : Enum
    {
        /// <summary>
        /// Gets or sets the screens that this element is shown on.
        /// </summary>
        public T Screens { get; set; }
    }

    /// <summary>
    /// Provides a wrapper that enables screen functionality for elements that do not normally support.
    /// </summary>
    /// <typeparam name="TEnum">The class of the enum.</typeparam>
    /// <typeparam name="TWrapper">The element to wrap around.</typeparam>
    public class AsScreen<TEnum, TWrapper> : IScreenElement<TEnum>
        where TEnum : Enum
        where TWrapper : IElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsScreen{TEnum, TWrapper}"/> class.
        /// </summary>
        /// <param name="screens">The screens that this element is visible on.</param>
        /// <param name="inner">The element to wrap around.</param>
        public AsScreen(TEnum screens, TWrapper inner)
        {
            Screens = screens;
            Inner = inner;
        }

        /// <summary>
        /// Gets the element that this is wrapping around.
        /// </summary>
        public TWrapper Inner { get; }

        /// <summary>
        /// Gets or sets the screens that this should display on.
        /// </summary>
        public TEnum Screens { get; set; }

        /// <inheritdoc/>
        public bool Enabled { get => Inner.Enabled; set => Inner.Enabled = value; }

        /// <inheritdoc/>
        public ParsedData ParsedData => Inner.ParsedData;

        /// <inheritdoc/>
        public float Position { get => Inner.Position; set => Inner.Position = value; }

        /// <inheritdoc/>
        public int ZIndex { get => Inner.ZIndex; set => Inner.ZIndex = value; }
    }

    /// <summary>
    /// Represents a <see cref="DynamicElement"/> that is tied to a number of screens.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    public class ScreenSetElement<T> : SetElement, IScreenElement<T>
        where T : Enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenSetElement{T}"/> class.
        /// </summary>
        /// <param name="screens">The screens that this element is visible on.</param>
        /// <param name="position">The scaled position of the element, where 0 is the bottom of the screen and 1000 is the top.</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        /// <param name="content">The content to set the element to.</param>
        public ScreenSetElement(T screens, float position, int zIndex = 0, string content = "")
            : base(position, zIndex, content)
        {
            Screens = screens;
        }

        /// <inheritdoc/>
        public T Screens { get; set; }
    }

    /// <summary>
    /// Represents a <see cref="DynamicElement"/> that it is tied to a number of screens.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    public class ScreenDynamicElement<T> : DynamicElement, IScreenElement<T>
        where T : Enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenDynamicElement{T}"/> class.
        /// </summary>
        /// <param name="contentGetter">A delegate returning the new content that will be ran every time the display is updated.</param>
        /// <param name="screens">The screens that this element is visible on.</param>
        /// <param name="position">The scaled position of the element, where 0 is the bottom of the screen and 1000 is the top.</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        public ScreenDynamicElement(GetContent contentGetter, T screens, float position, int zIndex = 0)
            : base(contentGetter, position, zIndex)
        {
            Screens = screens;
        }

        /// <inheritdoc/>
        public T Screens { get; set; }
    }

    /// <summary>
    /// Represents a non-cached element that evaluates and parses a function when getting its content.
    /// </summary>
    /// <remarks>
    /// The content of this element is re-evaluated by calling a function every time the display is updated. This makes it suitable for scenarios where you need to have information constantly updated. For example, you may use this to display the health of SCPs in an SCP list.
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicElement"/> class.
        /// </summary>
        /// <param name="contentGetter">A delegate returning the new content that will be ran every time the display is updated.</param>
        /// <param name="position">The scaled position of the element, where 0 is the bottom of the screen and 1000 is the top.</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        public DynamicElement(GetContent contentGetter, float position, int zIndex = 0)
        {
            ContentGetter = contentGetter;
        }

        /// <summary>
        /// Gets or sets a method that returns the new content and is called every time the display is updated.
        /// </summary>
        public GetContent ContentGetter { get; set; }

        /// <inheritdoc/>
        public bool Enabled { get; set; } = true;

        /// <inheritdoc/>
        public float Position { get; set; }

        /// <inheritdoc/>
        public int ZIndex { get; set; }

        /// <inheritdoc/>
        public ParsedData ParsedData => Constants.Parser.Parse(ContentGetter());
    }

    /// <summary>
    /// Represents a simple cached element with settable content.
    /// </summary>
    public class SetElement : IElement, ISettable
    {
        private ParsedData cachedParsedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetElement"/> class.
        /// </summary>
        /// <param name="position">The scaled position of the element, where 0 is the bottom of the screen and 1000 is the top.</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        /// <param name="content">The content to set the element to.</param>
        public SetElement(float position, int zIndex = 0, string content = "")
        {
            Position = position;
            cachedParsedData = Constants.Parser.Parse(content);
        }

        /// <inheritdoc/>
        public ParsedData ParsedData => cachedParsedData;

        /// <inheritdoc/>
        public bool Enabled { get; set; } = true;

        /// <inheritdoc/>
        public float Position { get; set; }

        /// <inheritdoc/>
        public int ZIndex { get; set; }

        /// <summary>
        /// Sets the content of this element.
        /// </summary>
        /// <param name="content">The text to set the content to (will be parsed).</param>
        public virtual void Set(string content)
        {
            cachedParsedData = Constants.Parser.Parse(content);
        }
    }

    /// <summary>
    /// Represents the base class for all elements.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not this element is enabled and will show.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the data used for parsing.
        /// </summary>
        public ParsedData ParsedData { get; }

        /// <summary>
        /// Gets or sets the position of the element on a scale from 0-1000, where 0 represents the bottom of the screen and 1000 represents the top.
        /// </summary>
        public float Position { get; set; }

        /// <summary>
        /// Gets or sets the priority of the hint (determining if it shows above another hint).
        /// </summary>
        public int ZIndex { get; set; }
    }
}