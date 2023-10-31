namespace RueI.Records
{
    /// <summary>
    /// Defines a record that contains information used for displaying multiple elements.
    /// </summary>
    /// <param name="Content">The element's content.</param>
    /// <param name="Offset">The offset that should be applied. Equivalent to the total linebreaks within the element.</param>
    public record struct ParsedData(string Content, float Offset);
}
