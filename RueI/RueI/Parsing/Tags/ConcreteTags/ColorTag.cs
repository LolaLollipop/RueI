namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;

    /// <summary>
    /// Provides a way to handle color tags.
    /// </summary>
    public class ColorTag : RichTextTag
    {
        /// <inheritdoc/>
        public override string[] Names { get; } = { "color" };

        /// <inheritdoc/>
        public override TagStyle TagStyle { get; } = TagStyle.ValueParam;

        /// <inheritdoc/>
        public override bool HandleTag(ParserContext context, string param)
        {
            if (param.StartsWith("#"))
            {
                if (!Constants.ValidColorSizes.Contains(param.Length - 1))
                {
                    return false;
                }
            }
            else
            {
                string? unquoted = TagHelpers.ExtractFromQuotations(param);
                if (unquoted == null || !Constants.Colors.Contains(unquoted))
                {
                    return false;
                }
            }

            context.ResultBuilder.Append($"<color={param}>");
            context.AddEndingTag<CloseSizeTag>();
            return true;
        }
    }
}
