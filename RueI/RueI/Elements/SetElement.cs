namespace RueI.Elements;

using RueI.Elements.Interfaces;
using RueI.Parsing;
using RueI.Parsing.Records;

/// <summary>
/// Represents a simple cached element with settable content.
/// </summary>
public class SetElement : Element, ISettable
{
    private ParsedData data;

    /// <summary>
    /// Initializes a new instance of the <see cref="SetElement"/> class.
    /// </summary>
    /// <param name="position">The scaled position of the element, where 0 is the bottom of the screen and 1000 is the top.</param>
    /// <param name="content">The content to set the element to.</param>
    public SetElement(float position, string content = "")
        : base(position)
    {
        Position = position;
        data = Parser.Parse(content);
    }

    /// <summary>
    /// Sets the content of this element.
    /// </summary>
    /// <param name="content">The text to set the content to (will be parsed).</param>
    public virtual void Set(string content)
    {
        data = Parser.Parse(content);
    }

    /// <inheritdoc/>
    protected internal override ParsedData GetParsedData() => data;
}