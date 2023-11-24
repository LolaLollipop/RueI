namespace RueI.Elements;

using RueI.Elements.Delegates;
using RueI.Parsing.Records;

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