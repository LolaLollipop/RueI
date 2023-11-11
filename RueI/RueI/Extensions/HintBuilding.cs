namespace RueI.Extensions.HintBuilding
{
    using System.Drawing;
    using System.Text;
    using RueI.Enums;

    /// <summary>
    /// Provides extensions for working with collections.
    /// </summary>
    public static class HintBuilding
    {
        /// <summary>
        /// Converts a <see cref="Color"/> to a hex code string.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert.</param>
        /// <returns>The color as a hex code string.</returns>
        public static string ConvertToHex(Color color)
        {
            string alphaInclude = color.A switch
            {
                255 => string.Empty,
                _ => color.A.ToString("X2")
            };

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{alphaInclude}";
        }

        /// <summary>
        /// Adds a linebreak to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder AddLinebreak(this StringBuilder sb) => sb.Append('\n');

        /// <summary>
        /// Adds a size tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="size">The size to include in the size tag.</param>
        /// <param name="style">The measurement style of the size tag.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetSize(this StringBuilder sb, float size, MeasurementStyle style = MeasurementStyle.Pixels)
        {
            string format = style switch
            {
                MeasurementStyle.Percentage => "%",
                MeasurementStyle.Ems => "ems",
                _ => string.Empty
            };

            return sb.Append($"<size={size}{format}>");
        }

        /// <summary>
        /// Adds a line-height tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="size">The line height to include in the line-height tag.</param>
        /// <param name="style">The measurement style of the line-height tag.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetLineHeight(this StringBuilder sb, float size, MeasurementStyle style = MeasurementStyle.Pixels)
        {
            string format = style switch
            {
                MeasurementStyle.Percentage => "%",
                MeasurementStyle.Ems => "ems",
                _ => string.Empty
            };

            return sb.Append($"<line-height={size}{format}>");
        }

        /// <summary>
        /// Adds a color tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="color">The color to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetColor(this StringBuilder sb, Color color)
        {
            return sb.Append($"<color={ConvertToHex(color)}>");
        }

        /// <summary>
        /// Adds a color tag to a <see cref="StringBuilder"/> from RGBA values.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="r">The red (0-255) in the color.</param>
        /// <param name="g">The green (0-255) in the color.</param>
        /// <param name="b">The blue (0-255) in the color.</param>
        /// <param name="alpha">The optional alpha (0-255) of the color.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetColor(this StringBuilder sb, int r, int g, int b, int alpha = 255)
        {
            Color color = Color.FromArgb(r, g, b, alpha);
            return sb.SetColor(color);
        }

        /// <summary>
        /// Adds a color tag to a <see cref="StringBuilder"/> from RGBA values.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="r">The red (0-255) in the color.</param>
        /// <param name="g">The green (0-255) in the color.</param>
        /// <param name="b">The blue (0-255) in the color.</param>
        /// <param name="alpha">The optional alpha (0-255) of the color.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetColor(this StringBuilder sb, byte r, byte g, byte b, byte alpha = 255)
        {
            Color color = Color.FromArgb(r, g, b, alpha);
            return sb.SetColor(color);
        }

        /// <summary>
        /// Adds an alpha tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="alpha">The alpha (0-255) of the color.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetAlpha(this StringBuilder sb, byte alpha) => sb.Append($"<alpha={alpha:X2}>");

        /// <summary>
        /// Adds an alpha tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="alpha">The alpha (0-255) of the color.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetAlpha(this StringBuilder sb, int alpha) => sb.Append($"<alpha={alpha:X2}>");

        /// <summary>
        /// Adds a bold tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetBold(this StringBuilder sb) => sb.Append("<b>");

        /// <summary>
        /// Adds an italics tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetItalics(this StringBuilder sb) => sb.Append("<i>");

        /// <summary>
        /// Adds a strikethrough tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetStrikethrough(this StringBuilder sb) => sb.Append("<s>");

        /// <summary>
        /// Adds an underline tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetUnderline(this StringBuilder sb) => sb.Append("<u>");

        /// <summary>
        /// Adds an indent tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="indent">The indent size to include in the indent tag.</param>
        /// <param name="style">The measurement style of the indent tag.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetIndent(this StringBuilder sb, float indent, MeasurementStyle style = MeasurementStyle.Pixels)
        {
            string format = style switch
            {
                MeasurementStyle.Percentage => "%",
                MeasurementStyle.Ems => "ems",
                _ => string.Empty
            };

            return sb.Append($"<indent={indent}{format}>");
        }

        /// <summary>
        /// Adds a case tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <param name="caseStyle">The case to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder SetCase(this StringBuilder sb, CaseStyle caseStyle)
        {
            string format = caseStyle switch
            {
                CaseStyle.Uppercase => "allcaps",
                CaseStyle.Lowercase => "lowercase",
                _ => "smallcaps",
            };

            return sb.Append($"<{format}>");
        }

        /// <summary>
        /// Adds a closing color tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseColor(this StringBuilder sb) => sb.Append("</color>");

        /// <summary>
        /// Adds a closing alpha tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseAlpha(this StringBuilder sb) => sb.Append("</alpha>");

        /// <summary>
        /// Adds a closing size tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseSize(this StringBuilder sb) => sb.Append("</size>");

        /// <summary>
        /// Adds a closing line-height tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseLineHeight(this StringBuilder sb) => sb.Append("</line-height>");

        /// <summary>
        /// Adds a closing bold tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseBold(this StringBuilder sb) => sb.Append("</b>");

        /// <summary>
        /// Adds a closing italics tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseItalics(this StringBuilder sb) => sb.Append("</i>");

        /// <summary>
        /// Adds a closing strikethrough tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseStrikethrough(this StringBuilder sb) => sb.Append("</s>");

        /// <summary>
        /// Adds a closing underline tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseUnderline(this StringBuilder sb) => sb.Append("</u>");

        /// <summary>
        /// Adds a closing indent tag to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">The <see cref="StringBuilder"/> to use.</param>
        /// <returns>A reference to the original <see cref="StringBuilder"/>.</returns>
        public static StringBuilder CloseIndent(this StringBuilder sb) => sb.Append("</indent>");
    }
}