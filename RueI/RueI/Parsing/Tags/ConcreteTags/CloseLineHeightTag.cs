namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing line-height tags.
/// </summary>
[RichTextTag]
public class CloseLineHeightTag : ClosingTag<CloseLineHeightTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/line-height";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.CurrentLineHeight = 0;
    }
}
