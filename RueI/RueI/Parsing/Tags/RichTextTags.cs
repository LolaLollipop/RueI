namespace RueI.Parsing
{
    /// <summary>
    /// Defines the base class for all rich text tags.
    /// </summary>
    public abstract class RichTextTag
    {
        /// <summary>
        /// Gets the names of the rich text tag.
        /// </summary>
        public abstract string[] Names { get; }

        /// <summary>
        /// Gets a new param processor, if the tag has params.
        /// </summary>
        /// <returns>A new param processor, or null if the tag has no params.</returns>
        public abstract ParamProcessor? GetProcessor();
    }
}
