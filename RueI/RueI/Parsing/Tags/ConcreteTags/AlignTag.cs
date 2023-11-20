namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Enums;
using RueI.Records;

/// <summary>
/// Provides a way to handle scale tags.
/// </summary>
public class AlignTag : RichTextTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "scale" };

    /// <inheritdoc/>
    public override TagStyle TagStyle { get; } = TagStyle.ValueParam;

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, string content)
    {
        string? alignment = TagHelpers.ExtractFromQuotations(content);

        if (alignment == null || !Constants.Alignments.Contains(alignment))
        {
            return false;
        }

        context.ResultBuilder.Append($"<align={alignment}>");
        context.AddEndingTag<CloseAlignTag>();

        return true;
    }
}
