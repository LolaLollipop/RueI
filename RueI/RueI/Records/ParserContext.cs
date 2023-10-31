using RueI.Enums;
using System.Text;

namespace RueI.Records
{
    /// <summary>
    /// Represents the context of a parser, for parameters.
    /// </summary>
    /// <param name="ResultBuilder">The current line height.</param>
    /// <param name="CurrentLineHeight">The current line height.</param>
    /// <param name="CurrentLineWidth">The current width of the line that the parser is on. </param>
    /// <param name="Size">Represents the current utilized </param>
    /// <param name="NewOffset">The final offset of the element.</param>
    /// <param name="CurrentCSpace">The current additional spacing between characters.</param>
    /// <param name="ShouldParse">Whether or not tags are currently being parsed.</param>
    /// <param name="IsMonospace">Whether or not the text is currently monospace.</param>
    /// <param name="IsBold">Whether or not the text is currently bold.</param>
    /// <param name="CurrentCase">The current case of the text.</param>
    /// <param name="SizeTags">A stack containing all the nested sizes.</param>
    /// <param name="ColorTags">The number of color tags that is currently nested.</param>
    public record ParserContext(
        StringBuilder ResultBuilder,
        float CurrentLineHeight,
        float CurrentLineWidth,
        float Size,
        float NewOffset,
        float CurrentCSpace,
        bool ShouldParse,
        bool IsMonospace,
        bool IsBold,
        CaseStyle CurrentCase,
        Stack<float> SizeTags,
        int ColorTags
    );
}

/*
 * using RueI.Enums;
using System.Text;

namespace RueI.Records
{
    /// <summary>
    /// Represents the context of a parser, for parameters.
    /// </summary>
    /// <param name="ResultBuilder">The end result string builder.</param>
    /// <param name="CurrentLineHeight">The current line height.</param>
    /// <param name="CurrentLineWidth">The current width of the line that the parser is on. </param>
    /// <param name="Size">Represents the current utilized </param>
    /// <param name="NewOffset">The final offset of the element.</param>
    /// <param name="CurrentCSpace">The current additional spacing between characters.</param>
    /// <param name="ShouldParse">Whether or not tags are currently being parsed.</param>
    /// <param name="IsMonospace">Whether or not the text is currently monospace.</param>
    /// <param name="IsBold">Whether or not the text is currently bold.</param>
    /// <param name="CurrentCase">The current case of the text.</param>
    /// <param name="SizeTags">A stack containing all the nested sizes.</param>
    /// <param name="ColorTags">The number of color tags that is currently nested.</param>
    public record ParserContext {
        public StringBuilder ResultBuilder { get; set; }
        public float CurrentLineHeight { get; set; }
        public float CurrentLineWidth { get; set; }
        public float Size { get; set; }
        public float NewOffset { get; set; }
        public float CurrentCSpace { get; set; }
        public bool ShouldParse { get; set; }
        public bool IsMonospace { get; set; }
        public bool IsBold { get; set; }
        public CaseStyle CurrentCase { get; set; }
        public Stack<float> SizeTags { get; set; }
        public int ColorTag { get; set; }
    }
}
*/