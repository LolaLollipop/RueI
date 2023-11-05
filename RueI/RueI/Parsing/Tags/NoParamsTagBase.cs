namespace RueI.Parsing.Tags
{
    /// <summary>
    /// Defines the base class for all tags that do not have params.
    /// </summary>
    public abstract class NoParamsTagBase : RichTextTag
    {
        /// <summary>
        /// Does not return anything.
        /// </summary>
        /// <returns><see cref="null"/>.</returns>
        public sealed override ParamProcessor? GetProcessor() => null;

        /// <summary>
        /// Handles an instance of a tag without params.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        public abstract void HandleTag(ParserContext context);
    }
}