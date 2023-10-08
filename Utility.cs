namespace RueI
{
    public static class Ruetility
    {
        /// <summary>
        /// Cleans a string by wrapping it in noparses, and removes any noparse closer tags existing in it already.
        /// </summary>
        /// <param name="text">The string to clean.</param>
        /// <returns>The cleaned string.</returns>
        public static string GetCleanText(string text)
        {
            string cleanText = text.Replace("</noparse>", "</nopa​rse>"); // zero width space is inserted
            return $"<noparse>{cleanText}</noparse>";
        }
    }
}
