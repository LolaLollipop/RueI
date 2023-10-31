namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;
    using RueI.Records;

    /// <summary>
    /// Provides a way to handle size tags.
    /// </summary>
    public class SizeTag : MeasurementTagBase
    {
        private const string TAGFORMAT = "<size={0}>";

        /// <inheritdoc/>
        public override string[] Names { get; } = { "size" };

        /// <inheritdoc/>
        protected override ParserContext HandleTag(ParserContext oldContext, float measurement, MeasurementStyle style)
        {
            oldContext.SizeTags.Push(oldContext.Size);
            float value = style switch
            {
                MeasurementStyle.Percentage => measurement / 100 * Constants.DEFAULTSIZE,
                MeasurementStyle.Ems => measurement * Constants.EMSTOPIXELS,
                _ => measurement
            };

            oldContext.ResultBuilder.AppendFormat(TAGFORMAT, value);
            return oldContext with { Size = value };
        }
    }
}
