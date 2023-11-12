namespace RueI
{
    using NorthwoodLib.Pools;
    using RueI.Parsing;

    /// <summary>
    /// Builds <see cref="Parser"/>s.
    /// </summary>
    public sealed class ParserBuilder
    {
        private List<RichTextTag> tags = ListPool<RichTextTag>.Shared.Rent(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserBuilder"/> class.
        /// </summary>
        public ParserBuilder()
        {
        }

        /// <summary>
        /// Creates a new instance of a <see cref="RichTextTag"/> and adds it to the builder.
        /// </summary>
        /// <typeparam name="T">The type of the tag to create.</typeparam>
        /// <returns>A reference to this <see cref="ParserBuilder"/>.</returns>
        public ParserBuilder AddTag<T>()
            where T : RichTextTag, new()
        {
            T tag = new();
            tags.Add(tag);

            return this;
        }

        /// <summary>
        /// Adds all the <see cref="RichTextTag"/>s from a <see cref="Parser"/>.
        /// </summary>
        /// <param name="enumerable">The tags to add.</param>
        /// <returns>A reference to this <see cref="ParserBuilder"/>.</returns>
        public ParserBuilder AddTags(IEnumerable<RichTextTag> enumerable)
        {
            foreach (RichTextTag tag in enumerable)
            {
                tags.Add(tag);
            }

            return this;
        }

        /// <summary>
        /// Adds a certain number of <see cref="RichTextTag"/>s to this builder.
        /// </summary>
        /// <param name="firstTag">The first tag to add.</param>
        /// <param name="extraTags">The rest of the tags to add.</param>
        /// <returns>A reference to this <see cref="ParserBuilder"/>.</returns>
        public ParserBuilder AddTags(RichTextTag firstTag, params RichTextTag[] extraTags)
        {
            tags.Add(firstTag);
            AddTags(extraTags);

            return this;
        }

        /// <summary>
        /// Imports all the <see cref="RichTextTag"/>s from a <see cref="Parser"/>.
        /// </summary>
        /// <param name="parser">The parser to import from.</param>
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
        public Parser Build() => new(tags);
    }
}
