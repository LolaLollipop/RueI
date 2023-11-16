namespace RueI.Parsing.Tags.ConcreteTags
{
    /// <summary>
    /// Provides a way to handle line-height tags.
    /// </summary>
    public class CloseLineHeightTag : NoParamsTag
    {
        private const string TAGFORMAT = "</line-height>";

        /// <inheritdoc/>
        public override string[] Names { get; } = { "/line-height" };

        /// <inheritdoc/>
        public override bool HandleTag(ParserContext context)
        {
            context.CurrentLineHeight = Constants.DEFAULTHEIGHT;

            context.ResultBuilder.Append(TAGFORMAT);
            context.RemoveEndingTag<CloseLineHeightTag>();
            return true;
        }
    }
}
