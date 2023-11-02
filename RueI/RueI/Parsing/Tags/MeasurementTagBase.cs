namespace RueI.Parsing.Tags
{
    using RueI.Enums;
    using RueI.Records;

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

        protected abstract void HandleTag(ParserContext oldContext, float measurement, MeasurementStyle style);
    }
}
