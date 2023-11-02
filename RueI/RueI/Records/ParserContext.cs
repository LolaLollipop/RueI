using NorthwoodLib.Pools;
using RueI.Enums;
using System.Text;

namespace RueI.Records
{
    public class ParserContext : IDisposable
    {
        /// <summary>
        /// Gets the end result string builder.
        /// </summary>
        public StringBuilder ResultBuilder { get; } = StringBuilderPool.Shared.Rent();

        /// <summary>
        /// Gets the current tag buffer.
        /// </summary>
        public StringBuilder TagBuffer { get; } = StringBuilderPool.Shared.Rent();

        /// <summary>
        /// Gets or sets the final offset for the element as a whole.
        /// </summary>
        public float NewOffset { get; set; }

        /// <summary>
        /// Gets a stack containing all of the nested sizes.
        /// </summary>
        public Stack<float> SizeTags { get; } = new();

        /// <summary>
        /// Gets or sets the current line height of the parser.
        /// </summary>
        public float CurrentLineHeight { get; set; }

        /// <summary>
        /// Gets or sets the current line width of the parser.
        /// </summary>
        public float CurrentLineWidth { get; set; }

        /// <summary>
        /// Gets or sets the total width since a space.
        /// </summary>
        public float WidthSinceSpace { get; set; }

        /// <summary>
        /// Gets or sets the current character of the parser.
        /// </summary>
        public float Size { get; set; }

        /// <summary>
        /// Gets or sets the current additional character spacing of the parser.
        /// </summary>
        public float CurrentCSpace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parser should parse tags other than noparse.
        /// </summary>
        public bool ShouldParse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the characters are currently in monospace.
        /// </summary>
        public bool IsMonospace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not words are currently in no break.
        /// </summary>
        public bool NoBreak { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the characters are currently bold.
        /// </summary>
        public bool IsBold { get; set; }

        /// <summary>
        /// Gets or sets the current case of the parser.
        /// </summary>
        public CaseStyle CurrentCase { get; set; }

        /// <summary>
        /// Gets or sets the number of color tags that are nested.
        /// </summary>
        public int ColorTags { get; set; }

        /// <summary>
        /// Disposes this ParserContext, returning the string builder to the pool.
        /// </summary>
        public void Dispose()
        {
            StringBuilderPool.Shared.Return(ResultBuilder);
            StringBuilderPool.Shared.Return(TagBuffer);
        }
    }
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