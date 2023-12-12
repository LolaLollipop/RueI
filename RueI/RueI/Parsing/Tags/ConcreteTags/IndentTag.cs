namespace RueI.Parsing.Tags.ConcreteTags;

using RueI.Parsing.Enums;
using RueI.Parsing.Records;

/// <summary>
/// Provides a way to handle indent tags.
/// </summary>
[RichTextTag]
public class IndentTag : MeasurementTag
{
    /// <inheritdoc/>
    public override string[] Names { get; } = { "indent" };

    /// <inheritdoc/>
    public override bool HandleTag(ParserContext context, MeasurementInfo info)
    {
        float value = info.Style switch
        {
            MeasurementUnit.Percentage => info.Value / 100 * Constants.DISPLAYAREAWIDTH,
            MeasurementUnit.Ems => info.Value * Constants.EMSTOPIXELS,
            _ => info.Value
        };

        context.Indent = value;
        context.ResultBuilder.Append($"<indent={value}>");
        context.AddEndingTag<CloseIndentTag>();

        return true;
    }
}
