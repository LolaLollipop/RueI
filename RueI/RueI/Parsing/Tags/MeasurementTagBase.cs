namespace RueI.Parsing.Tags
{
    using RueI.Enums;

    /// <summary>
    /// Defines the base class for all tags that take in a measurement.
    /// </summary>
    public abstract class MeasurementTagBase : RichTextTag
    {
        /// <summary>
        /// Gets a new measurement tag processor for this tag.
        /// </summary>
        /// <returns>The new measurement tag processor.</returns>
        public override ParamProcessor? GetProcessor() => new MeasurementParamProcessor(HandleTag);

        /// <summary>
        /// Handles an instance of the measurement tag.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <param name="measurement">The value of the measurement.</param>
        /// <param name="style">The style of the measurement.</param>
        protected abstract void HandleTag(ParserContext context, float measurement, MeasurementStyle style);
    }
}
