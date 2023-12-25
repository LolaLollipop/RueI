namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing color tags.
/// </summary>
[RichTextTag]
public class CloseStrikethroughTag : ClosingTag<CloseStrikethroughTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/s";
}
