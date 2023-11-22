namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing scale tags.
/// </summary>
[RichTextTag]
public class CloseScaleTag : ClosingTag<CloseScaleTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/scale";
}
