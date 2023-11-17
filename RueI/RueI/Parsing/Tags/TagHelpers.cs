namespace RueI.Parsing.Tags;

/// <summary>
/// Provides a way to handle singletons of tags.
/// </summary>
/// <typeparam name="T">The <see cref="RichTextTag"/> type to share.</typeparam>
/// <remarks>
/// This class provides a way to guarantee that only one instance of a tag will ever be used by the parser,
/// since tags are not static to support inheritance but must act similar to it.
/// </remarks>
public static class TagHelpers
{
    // TODO: document
    public static string? ExtractFromQuotations(string str)
    {
        return (str.StartsWith("\""), str.EndsWith("\"")) switch
        {
            (true, true) => str.Substring(1, str.Length - 1),
            (false, true) => null,
            (true, false) => null,
            (false, false) => str,
        };
    }
}
