namespace RueI;

using System.Reflection;
using RueI.Parsing;
using RueI.Parsing.Tags;

/// <summary>
/// Provides the default and main <see cref="Parser"/> for RueI.
/// </summary>
public static class DefaultParser
{
    /// <summary>
    /// Gets the default <see cref="Parser"/>.
    /// </summary>
    public static Parser Parser { get; } = GetParser();

    private static Parser GetParser()
    {
        ParserBuilder builder = new();
        Assembly currentAssembly = typeof(DefaultParser).Assembly;

        MethodInfo addTag = typeof(ParserBuilder).GetMethod(nameof(ParserBuilder.AddTag));

        foreach (Type type in currentAssembly.GetTypes())
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