namespace RueI.Parsing.Records;

/// <summary>
/// Defines a record that contains information used for displaying multiple elements.
/// </summary>
/// <param name="content">The element's content.</param>
/// <param name="offset">The offset that should be applied. Equivalent to the total linebreaks within the element.</param>
public record ParsedData(string content, float offset);
