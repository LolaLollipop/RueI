using RueI.Enums;
using RueI.Records;
using System.Text;

namespace RueI.Parsing.Tags
{
    /// <summary>
    /// Defines the base class for all tags that take in a measurement.
    /// </summary>
    public abstract class NoParamsTag : RichTextTag
    {
        /// <summary>
        /// Gets a new measurement tag processor for this tag.
        /// </summary>
        /// <returns>The new measurement tag processor.</returns>
        public override ParamProcessor? GetProcessor() => new MeasurementParamProcessor(HandleTag);

        public abstract ParserContext HandleTag(ParserContext oldContext);
    }
}
s