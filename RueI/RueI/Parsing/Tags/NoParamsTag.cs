namespace RueI.Parsing;

using RueI.Enums;

/// <summary>
/// Defines the base class for all rich text tags.
/// </summary>
public abstract class NoParamsTag : RichTextTag
{
    /// <inheritdoc/>
    public sealed override TagStyle TagStyle { get; } = TagStyle.NoParams;

    /// <inheritdoc/>
    public sealed override bool HandleTag(ParserContext context, string parameters) => HandleTag(context, string.Empty);

    /// <summary>
    /// Applies this tag (without parameters) to a <see cref="ParserContext"/>.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    /// <returns>true if the tag is valid, otherwise false.</returns>
    /// <remarks>If the tag does not have parameters, <paramref name="parameters"/> will always be <see cref="string.Empty"/>.</remarks>
    public abstract bool HandleTag(ParserContext context);
}
