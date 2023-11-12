namespace RueI.Parsing
{
    using System.Text;
    using NorthwoodLib.Pools;
    using RueI.Enums;

    /// <summary>
    /// Provides information about TMP text at a certain point.
    /// </summary>
    /// <remarks>This class provides information necessary for individual character sizes, but does not provide the required information to calculate vertical offsets and total line widths. For that, use the derived class <see cref="ParserContext"/>.</remarks>
    public class TextInfo
    {
        /// <summary>
        /// Gets or sets the current line height of the parser.
        /// </summary>
        public float CurrentLineHeight { get; set; } = Constants.DEFAULTHEIGHT;

        /// <summary>
        /// Gets or sets the current character of the parser.
        /// </summary>
        public float Size { get; set; } = Constants.DEFAULTSIZE;

        /// <summary>
        /// Gets or sets the current additional character spacing of the text.
        /// </summary>
        public float CurrentCSpace { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether the characters are currently in monospace.
        /// </summary>
        public bool IsMonospace { get; set; } = false;

        /// <summary>
        /// Gets or sets the monospacing of the text.
        /// </summary>
        public float Monospacing { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether or not the characters are currently bold.
        /// </summary>
        public bool IsBold { get; set; } = false;

        /// <summary>
        /// Gets or sets the scale of the parser.
        /// </summary>
        public float Scale { get; set; } = 1;

        /// <summary>
        /// Gets or sets the current case of the parser.
        /// </summary>
        public CaseStyle CurrentCase { get; set; } = CaseStyle.Smallcaps;
    }
}