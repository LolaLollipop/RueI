namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;
    using RueI.Records;

    /// <summary>
    /// Provides a way to handle line-height tags.
    /// </summary>
    public class LineHeightTag : MeasurementTag
    {
        private const string TAGFORMAT = "<line-height={0}>";

        /// <inheritdoc/>
        public override string[] Names { get; } = { "line-height" };

        /// <inheritdoc/>
        public override bool HandleTag(ParserContext context, MeasurementInfo info)
        {
            var (value, style) = info;

            float convertedValue = style switch
            {
                MeasurementUnit.Percentage => value / 100 * Constants.DEFAULTSIZE,
                MeasurementUnit.Ems => value * Constants.EMSTOPIXELS,
                _ => value
            };

            context.CurrentLineHeight = convertedValue;
            context.ResultBuilder.AppendFormat(TAGFORMAT, convertedValue);

            context.AddEndingTag<CloseLineHeightTag>();

            return true;
        }
    }
}
