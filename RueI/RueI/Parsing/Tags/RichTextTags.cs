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
        /// Returns a value indicating whether or not the <see cref="char"/> is a valid delimiter for this tag.
        /// </summary>
        /// <returns><see cref="true"/> if the char is a valid delimiter, otherwise <see cref="false"/>.</returns>
        /// <param name="ch">The <see cref="char"/> to check.</param>
        public abstract bool IsValidDelimiter(char ch);
    }
}
