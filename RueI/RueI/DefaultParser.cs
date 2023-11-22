namespace RueI;

using System.Reflection;
using RueI.Parsing;
using RueI.Parsing.Tags;

/// <summary>
/// Provides the default and main <see cref="RueI.Parser"/> for RueI.
/// </summary>
public static class DefaultParser
{
    /// <summary>
    /// Gets the default <see cref="RueI.Parser"/>.
    /// </summary>
    public static Parser Parser { get; } = GetParser(typeof(DefaultParser).Assembly);

    /// <summary>
    /// Gets a new <see cref="RueI.Parser"/> from an assembly by getting all of the <see cref="RichTextTagAttribute"/> classes.
    /// </summary>
    /// <returns>A new <see cref="RueI.Parser"/>.</returns>
    /// <remarks>This method is used for unit testing.</remarks>
    private static Parser GetParser(Assembly assembly)
    {
        ParserBuilder builder = new();

        MethodInfo addTag = typeof(ParserBuilder).GetMethod(nameof(ParserBuilder.AddTag));

        foreach (Type type in assembly.GetTypes())
        {
            if (type.GetCustomAttributes(typeof(RichTextTagAttribute), true).Any() && type.IsSubclassOf(typeof(RichTextTag)))
            {
                MethodInfo generic = addTag.MakeGenericMethod(type);
                generic.Invoke(builder, Array.Empty<object>());
            }
        }

        return builder.Build();
    }
}