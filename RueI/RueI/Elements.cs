namespace RueI;

using RueI.Delegates;
using RueI.Interfaces;
using RueI.Records;

/// <summary>
/// Represents a non-cached element that evaluates and parses a function when getting its content.
/// </summary>
/// <remarks>
/// The content of this element is re-evaluated by calling a function every time the display is updated. This makes it suitable for scenarios where you need to have information constantly updated. For example, you may use this to display the health of SCPs in an SCP list.
/// </remarks>
public class DynamicElement : IElement
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
    public Parser Parser { get; set; } = Parser.DefaultParser;

    /// <inheritdoc/>
    public ParsedData ParsedData => Parser.Parse(ContentGetter());
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
    /// <param name="content">The content to set the element to.</param>
    public SetElement(float position, string content = "")
    {
        Position = position;
        cachedParsedData = Parser.Parse(content);
    }

    /// <inheritdoc/>
    public ParsedData ParsedData => cachedParsedData;

    /// <inheritdoc/>
    public bool Enabled { get; set; } = true;

    /// <inheritdoc/>
    public float Position { get; set; }

    /// <inheritdoc/>
    public int ZIndex { get; set; } = 0;

    /// <inheritdoc/>
    public Parser Parser { get; set; } = Parser.DefaultParser;

    /// <summary>
    /// Sets the content of this element.
    /// </summary>
    /// <param name="content">The text to set the content to (will be parsed).</param>
    public virtual void Set(string content)
    {
        cachedParsedData = Parser.Parse(content);
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
    /// <remarks>This contains information used to ensure that multiple elements can be displayed at once. To obtain this, you should almost always use <see cref="Parser.Parse"/>.</remarks>
    public ParsedData ParsedData { get; }

    /// <summary>
    /// Gets or sets the position of the element on a scale from 0-1000, where 0 represents the bottom of the screen and 1000 represents the top.
    /// </summary>
    public float Position { get; set; }

    /// <summary>
    /// Gets or sets the priority of the hint (determining if it shows above another hint).
    /// </summary>
    public int ZIndex { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Parser"/> currently in use by this <see cref="IElement"/>.
    /// </summary>
    /// <remarks>Implementations should default this to <see cref="Parser.DefaultParser"/>.</remarks>
    public Parser Parser { get; set; }
}