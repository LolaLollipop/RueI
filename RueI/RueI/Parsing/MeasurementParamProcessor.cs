namespace RueI.Parsing
{
    using System.Text;
    using NorthwoodLib.Pools;
    using RueI.Enums;

    using RueI.Records;

    /// <summary>
    /// Processes measurement params for a tag.
    /// </summary>
    public class MeasurementParamProcessor : ParamProcessor
    {
        /// <summary>
        /// Represents an action to execute when processed.
        /// </summary>
        /// <param name="sb">The parser's buffer.</param>
        /// <param name="context">The context of the parser.</param>
        /// <param name="value">The measurement value.</param>
        /// <param name="style">The measurement style.</param>
        public delegate ParserContext MeasurementTagHandler(ParserContext context, float value, MeasurementStyle style);

        private MeasurementTagHandler tagHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasurementParamProcessor"/> class.
        /// </summary>
        /// <param name="processor">The action to execute when processed.</param>
        public MeasurementParamProcessor(MeasurementTagHandler processor)
        {
            tagHandler = processor;
        }

        /// <inheritdoc/>
        protected override bool Finish(Func<ParserContext> lazyContext, Action<ParserContext> lazyUnload)
        {
            StringBuilder paramBuffer = StringBuilderPool.Shared.Rent();
            string text = StringBuilderPool.Shared.ToStringReturn(buffer);
            MeasurementStyle style = MeasurementStyle.Pixels;
            bool hasPeriod = false;

            foreach (char ch in text)
            {
                if (ch == 'e')
                {
                    style = MeasurementStyle.Ems;
                    break;
                }
                else if (ch == '%')
                {
                    style = MeasurementStyle.Percentage;
                    break;
                }
                else if (ch == 'p') // pixels
                {
                    break;
                }
                else if (ch == '.')
                {
                    if (!hasPeriod)
                    {
                        hasPeriod = true;
                        paramBuffer.Append('.');
                    }
                }
                else if (char.IsDigit(ch))
                {
                    paramBuffer.Append(ch);
                }
            }

            string bufferString = StringBuilderPool.Shared.ToStringReturn(paramBuffer);
            if (float.TryParse(bufferString, out float result))
            {
                ParserContext newContext = tagHandler(lazyContext(), result, style);
                lazyUnload(newContext);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
