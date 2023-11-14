namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;

    /// <summary>
    /// Provides a way to handle closing size tags.
    /// </summary>
    public class ResetTag : NoParamsTagBase
    {
        /// <inheritdoc/>
        public override string[] Names { get; } = { "/reset" };

        /// <inheritdoc/>
        public override void HandleTag(ParserContext context)
        {
            if (context.Scale != 1)
            {

            }
        }
    }
}
