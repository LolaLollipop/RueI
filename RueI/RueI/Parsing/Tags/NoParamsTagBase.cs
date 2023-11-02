using RueI.Enums;
using RueI.Records;
using System.Text;

namespace RueI.Parsing.Tags
{
    /// <summary>
    /// Defines the base class for all tags that take in a measurement.
    /// </summary>
    public abstract class NoParamsTagBase : RichTextTag
    {
        /// <summary>
        /// Gets a new measurement tag processor for this tag.
        /// </summary>
        /// <returns>The new measurement tag processor.</returns>
        public sealed override ParamProcessor? GetProcessor() => null;

        /// <summary>
        /// Handles an instance of the tag.
        /// </summary>
        /// <param name="oldContext">The old context of the parser.</param>
        /// <returns>The modified context.</returns>
        public abstract void HandleTag(ParserContext oldContext);
    }
}