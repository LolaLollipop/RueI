namespace RueI.Parsing.Tags.ConcreteTags
{
    using RueI.Enums;

    /// <summary>
    /// Provides a way to handle size tags.
    /// </summary>
    public class SpriteTag : ParamsTagBase
    {

        /// <inheritdoc/>
        public override string[] Names { get; } = { "sprite" };

        /// <inheritdoc/>
        public override bool IsValidDelimiter(char ch) => ch == '=' || ch == ' ';

        /// <inheritdoc/>
        public override bool HandleTag(ParserContext context, char delimiter, string content)
        {
            string attributed = delimiter switch
            {
                '=' => GetUntilChar(content, ' '),
                _ => content // get until start of attribute pairs
            };

            return true;
        }

        private static bool IsValidSprite(int index) => index >= 0 && index <= 15;

        private static string GetUntilChar(string content, char ch)
        {
            int index = content.IndexOf(ch);
            return index switch
            {
                >= 0 => content.Substring(0, index),
                _ => string.Empty
            };
        }
    }
}
