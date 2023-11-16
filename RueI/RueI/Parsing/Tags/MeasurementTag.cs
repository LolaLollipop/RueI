namespace RueI.Parsing.Tags
{
    using RueI.Enums;
    using RueI.Records;

    /// <summary>
    /// Defines the base class for all tags that only take in a measurement.
    /// </summary>
    public abstract class MeasurementTag : RichTextTag
    {
        /// <inheritdoc/>
        public sealed override TagStyle TagStyle { get; } = TagStyle.ValueParam;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public sealed override bool HandleTag(ParserContext context, string content)
        {
            if (MeasurementInfo.TryParse(content, out MeasurementInfo info))
            {
                return HandleTag(context, info);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Handles an instance of this tag with <see cref="MeasurementInfo"/>.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <param name="info">The information about the measurement.</param>
        /// <returns>true if the tag is valid, otherwise false.</returns>
        public abstract bool HandleTag(ParserContext context, MeasurementInfo info);
    }
}
