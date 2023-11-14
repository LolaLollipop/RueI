namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;
    using RueI.Records;

    /// <summary>
    /// Provides a way to handle line-height tags.
    /// </summary>
    public class CloseLineHeightTag : NoParamsTagBase
    {
        private const string TAGFORMAT = "</line-height>";

        /// <inheritdoc/>
        public override string[] Names { get; } = { "/line-height" };

        /// <inheritdoc/>
        public override void HandleTag(ParserContext context)
        {
            context.CurrentLineHeight = Constants.DEFAULTHEIGHT;

            context.ResultBuilder.Append(TAGFORMAT);
            context.ClosingTags.Remove(this);
        }
    }
}
