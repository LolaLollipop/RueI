namespace RueI;

using RueI.Elements;
using RueI.Elements.Interfaces;
using RueI.Parsing.Records;

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