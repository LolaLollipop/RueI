namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing line-height tags.
/// </summary>
[RichTextTag]
public class CloseNobrTag : ClosingTag<CloseNobrTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/nobr";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.NoBreak = false;
    }
}
