namespace RueI.Enums
{
    /// <summary>
    /// Represents the state of the parser.
    /// </summary>
    internal enum ParserState
    {
        /// <summary>
        /// Indicates that the parser is currently looking for a tag to start parsing.
        /// </summary>
        CollectingTags,

        /// <summary>
        /// Indicates that the parser is currently descending the tag tree for a tag.
        /// </summary>
        DescendingTag,

        /// <summary>
        /// Indicates that the parser is currently collecting characters for a color param tag.
        /// </summary>
        CollectingColorParams,

        /// <summary>
        /// Indicates that the parser is currently collecting characters for a measurement param tag.
        /// </summary>
        CollectingMeasureParams,
    }
}
