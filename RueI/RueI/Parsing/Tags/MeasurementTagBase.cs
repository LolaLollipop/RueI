namespace RueI.Parsing.Tags
{
    using System.Text;
    using NorthwoodLib.Pools;
    using RueI.Enums;
    using RueI.Records;

    /// <summary>
    /// Defines the base class for all tags that only take in a measurement.
    /// </summary>
    public abstract class MeasurementTagBase : ParamsTagBase
    {
        /// <inheritdoc/>
        public override bool IsValidDelimiter(char ch) => ch == '=';

        /// <inheritdoc/>
        public override bool HandleTag(ParserContext context, char delimiter, string content)
        {
            if (!IsValidDelimiter(delimiter))
            {
                return false;
            }

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
        /// Handles an instance of this tag.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <param name="info">The information about the measurement.</param>
        /// <returns><see cref="true"/> if the tag is valid, otherwise <see cref="false"/>.</returns>
        public abstract bool HandleTag(ParserContext context, MeasurementInfo info);
    }
}
