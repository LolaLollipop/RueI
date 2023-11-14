namespace RueI.Parsing.Tags.ConcreteTags
{
    /// <summary>
    /// Provides a way to handle singletons of tags.
    /// </summary>
    /// <typeparam name="T">The <see cref="RichTextTag"/> type to share.</typeparam>
    public static class SharedTag<T>
        where T : RichTextTag, new()
    {
        /// <summary>
        /// Gets the shared singleton for this <see cref="RichTextTag"/>.
        /// </summary>
        public static T Singleton { get; } = new();
    }
}
