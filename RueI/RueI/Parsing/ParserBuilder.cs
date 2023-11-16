namespace RueI
{
    using NorthwoodLib.Pools;
    using RueI.Parsing;
    using RueI.Parsing.Tags;

    /// <summary>
    /// Builds <see cref="Parser"/>s.
    /// </summary>
    public sealed class ParserBuilder
    {
        private readonly List<RichTextTag> currentTags = ListPool<RichTextTag>.Shared.Rent(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserBuilder"/> class.
        /// </summary>
        public ParserBuilder()
        {
        }

        /// <summary>
        /// Gets the <see cref="SharedTag{T}"/> of a <see cref="RichTextTag"/> type and adds it to the builder.
        /// </summary>
        /// <typeparam name="T">The type of the tag to create.</typeparam>
        /// <returns>A reference to this <see cref="ParserBuilder"/>.</returns>
        public ParserBuilder AddTag<T>()
            where T : RichTextTag, new()
        {
            T tag = SharedTag<T>.Singleton;
            currentTags.Add(tag);

            return this;
        }

        /// <summary>
        /// Imports all of the <see cref="RichTextTag"/>s from a <see cref="Parser"/>, adding it to the builder.
        /// </summary>
        /// <param name="parser">The <see cref="Parser"/> to import the tags from.</param>
        /// <returns>A reference to this <see cref="ParserBuilder"/>.</returns>
        public ParserBuilder ImportFrom(Parser parser)
        {
            parser.ExportTo(this);
            return this;
        }

        /// <summary>
        /// Builds this <see cref="ParserBuilder"/> into a <see cref="Parser"/>.
        /// </summary>
        /// <returns>The built <see cref="Parser"/>.</returns>
        public Parser Build() => new(currentTags);

        /// <summary>
        /// Adds all of the tags from an <see cref="IEnumerable{RichTextTag}"/>.
        /// </summary>
        /// <param name="tags">The tags to add.</param>
        internal void AddTags(IEnumerable<RichTextTag> tags)
        {
            foreach (RichTextTag tag in tags)
            {
                currentTags.Add(tag);
            }
        }
    }
}
