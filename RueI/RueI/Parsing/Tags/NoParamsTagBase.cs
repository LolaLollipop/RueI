namespace RueI.Parsing.Tags
{
    /// <summary>
    /// Defines the base class for all tags that do not have params.
    /// </summary>
    public abstract class NoParamsTagBase : RichTextTag
    {
        /// <inheritdoc/>
        public override bool IsValidDelimiter(char ch) => ch == '>';

        /// <summary>
        /// Handles an instance of a tag without params.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        public abstract void HandleTag(ParserContext context);
    }
}