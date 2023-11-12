namespace RueI.Parsing.Tags
{
    /// <summary>
    /// Defines the base class for all tags that take in any params.
    /// </summary>
    public abstract class ParamsTagBase : RichTextTag
    {
        /// <summary>
        /// Handles an instance of this tag.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <param name="delimiter">The delimiter of the tag.</param>
        /// <param name="content">The content of the tag.</param>
        /// <returns><see cref="true"/> if the tag is valid, otherwise <see cref="false"/>.</returns>
        public abstract bool HandleTag(ParserContext context, char delimiter, string content);
    }
}
