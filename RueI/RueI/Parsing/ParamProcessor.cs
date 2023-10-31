namespace RueI.Parsing
{
    using System.Text;
    using NorthwoodLib.Pools;
    using RueI.Records;

    /// <summary>
    /// Defines the base class for a parameter processor for a tag.
    /// </summary>
    public abstract class ParamProcessor
    {
        protected StringBuilder buffer = StringBuilderPool.Shared.Rent();
        protected bool isFinished = false;

        /// <summary>
        /// Provides a character to the processor.
        /// </summary>
        /// <param name="ch">The character to prvoide.</param>
        public void Add(char ch)
        {
            buffer.Append(ch);
        }

        /// <summary>
        /// Signals to the procesor that the tags are finished.
        /// </summary>
        /// <param name="lazyContext">A function that lazily returns the context of the parser.</param>
        /// <param name="lazyUnload">An action that lazily hanldes the new context of the parser.</param>
        /// <param name="unloaded">If <see cref="false"/>, the buffer of the param parser.</param>
        /// <returns><see cref="true"/> if parsing was successful, otherwise <see cref="false"/>.<returns>
        public abstract bool Finish(Func<ParserContext> lazyContext, Action<ParserContext> lazyUnload, out string? unloaded);
    }
}
