namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing noparse tags.
/// </summary>
[RichTextTag]
public class CloseNoparseTag : ClosingTag<CloseNoparseTag>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/noparse";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.ShouldParse = true;
    }
}
