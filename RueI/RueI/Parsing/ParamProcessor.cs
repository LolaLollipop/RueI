namespace RueI.Parsing
{
    using System.Text;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Defines the base class for a parameter processor for a tag.
    /// </summary>
    public abstract class ParamProcessor : IDisposable
    {
        protected StringBuilder buffer = StringBuilderPool.Shared.Rent();

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
        /// <param name="context">The context of the parser.</param>
        /// <param name="unloaded">If <see cref="false"/>, the buffer of the param parser.</param>
        /// <returns><see cref="true"/> if parsing was successful, otherwise <see cref="false"/>.<returns>
        public bool GetFinishResult(ParserContext context, out string? unloaded)
        {
            bool wasSuccessful = Finish(context);
            if (wasSuccessful)
            {
                unloaded = null;
                return true;
            }
            else
            {
                unloaded = buffer.ToString();
                return false;
            }
        }

        /// <summary>
        /// Disposes this param processor and returns the string builder to the pool.
        /// </summary>
        public void Dispose()
        {
            StringBuilderPool.Shared.Return(buffer);
        }

        /// <summary>
        /// Signals to the procesor that the tags are finished.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <returns><see cref="true"/> if parsing was successful, otherwise <see cref="false"/>.<returns>
        protected abstract bool Finish(ParserContext context);
    }
}
