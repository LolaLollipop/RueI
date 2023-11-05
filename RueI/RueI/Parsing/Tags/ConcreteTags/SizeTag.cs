namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;

    /// <summary>
    /// Provides a way to handle size tags.
    /// </summary>
    public class SizeTag : MeasurementTagBase
    {
        private const string TAGFORMAT = "<size={0}>";

        /// <inheritdoc/>
        public override string[] Names { get; } = { "size" };

        /// <inheritdoc/>
        protected override void HandleTag(ParserContext context, float measurement, MeasurementStyle style)
        {
            context.SizeTags.Push(context.Size);
            float value = style switch
            {
                MeasurementStyle.Percentage => measurement / 100 * Constants.DEFAULTSIZE,
                MeasurementStyle.Ems => measurement * Constants.EMSTOPIXELS,
                _ => measurement
            };

            context.ResultBuilder.AppendFormat(TAGFORMAT, value);
        }
    }
}
