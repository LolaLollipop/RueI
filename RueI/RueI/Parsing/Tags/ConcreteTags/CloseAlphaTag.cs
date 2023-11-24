namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing alpha tags.
/// </summary>
[RichTextTag]
public class CloseAlphaTag : ClosingTag<CloseAlphaTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/alpha";
}
