using RueI.Parsing;

namespace RueI.Records
{
    internal record ParserNode
    {
        public Dictionary<char, ParserNode> Branches { get; } = new();
        public RichTextTag? Tag { get; set; }

        public ParserNode(RichTextTag? tag = null)
        {
            Tag = tag;
        }
    }
}
