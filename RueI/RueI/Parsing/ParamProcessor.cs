namespace RueI.Parsing
{
    /// <summary>
    /// Defines the base class for a parameter processor for a tag.
    /// </summary>
    public abstract class ParamProcessor
    {
        /// <summary>
        /// Signals to the procesor that the tags are finished.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <param name="paramContent">The content of the parser.</param>
        /// <returns><see cref="true"/> if parsing was successful, otherwise <see cref="false"/>.<returns>
        public abstract bool Finish(ParserContext context, string paramContent);
    }
}
