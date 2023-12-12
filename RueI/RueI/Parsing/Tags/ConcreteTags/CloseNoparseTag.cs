namespace RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a way to handle closing line-height tags.
/// </summary>
[RichTextTag]
public class CloseNoparse : ClosingTag<CloseNoparse>
{
    /// <inheritdoc/>
    public override string Name { get; } = "/noparse";

    /// <inheritdoc/>
    protected override void ApplyTo(ParserContext context)
    {
        context.ShouldParse = true;
    }
}
