namespace RueI.Parsing
{
    using System.Text;
    using NorthwoodLib.Pools;
    using RueI.Enums;

    /// <summary>
    /// Processes measurement params for a tag.
    /// </summary>
    public class MeasurementParamProcessor : ParamProcessor
    {
        /// <summary>
        /// Represents an action to execute when processed.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <param name="value">The measurement value.</param>
        /// <param name="style">The measurement style.</param>
        public delegate void MeasurementTagHandler(ParserContext context, float value, MeasurementStyle style);

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
        protected override bool Finish(ParserContext context)
        {
            StringBuilder paramBuffer = StringBuilderPool.Shared.Rent(25);
            MeasurementStyle style = MeasurementStyle.Pixels;

            bool hasPeriod = false;

            foreach (char ch in StringBuilderPool.Shared.ToStringReturn(this.buffer))
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
                tagHandler(context, result, style);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
