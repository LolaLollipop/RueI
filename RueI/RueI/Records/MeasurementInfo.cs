namespace RueI.Records
{
    using System.Text;
    using NorthwoodLib.Pools;
    using RueI.Enums;

    /// <summary>
    /// Defines a record that contains information about measurement info.
    /// </summary>
    /// <param name="Value">The value of the measurement.</param>
    /// <param name="Style">The style of the measurement.</param>
    public record struct MeasurementInfo(float Value, MeasurementStyle Style)
    {
        /// <summary>
        /// Attempts to extract a <see cref="MeasurementInfo"/> from a string.
        /// </summary>
        /// <param name="content">The content to parse.</param>
        /// <param name="info">The returned info, if <see cref="true"/>.</param>
        /// <returns><see cref="true"/> if the string was valid, otherwise <see cref="false"/>.</returns>
        public static bool TryParse(string content, out MeasurementInfo info)
        {
            StringBuilder paramBuffer = StringBuilderPool.Shared.Rent(25);
            MeasurementStyle style = MeasurementStyle.Pixels;

            bool hasPeriod = false;

            foreach (char ch in content)
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
                info = new(result, style);
                return true;
            }
            else
            {
                info = default;
                return false;
            }

        }
    }
}
