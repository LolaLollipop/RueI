namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;

    /// <summary>
    /// Provides a way to handle closing size tags.
    /// </summary>
    public class CloseSizeTag : NoParamsTagBase
    {
        private const string TAGFORMAT = "</size>";

        /// <inheritdoc/>
        public override string[] Names { get; } = { "/size" };

        /// <inheritdoc/>
        public override void HandleTag(ParserContext context)
        {
            if (context.SizeTags.Any())
            {
                context.Size = context.SizeTags.Pop();
            }
            else
            {
                context.Size = Constants.DEFAULTSIZE;
            }

            context.ResultBuilder.Append(TAGFORMAT);
        }
    }
}
