namespace RueI;

using System.Collections.ObjectModel;
using RueI.Parsing;
using RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Provides a variety of constant values.
/// </summary>
/// <remarks>This class is mosty designed for internal use within RueI. However, they can still be useful for external use.</remarks>
public static class Constants
{
    /// <summary>
    /// Gets the default height if a line-height is not provided.
    /// </summary>
    /// <remarks>Approximate.</remarks>
    public const float DEFAULTHEIGHT = 40.665f; // in pixels. this is barely approximate

    /// <summary>
    /// Gets the default size (in pixels) if a size is not provided.
    /// </summary>
    /// <remarks>Not approximate.</remarks>
    public const float DEFAULTSIZE = 34.7f; // in pixels. this is not approximate

    /// <summary>
    /// Gets the multiplier used to convert the size of a capital character to a smallcaps character.
    /// </summary>
    public const float CAPSTOSMALLCAPS = 0.8f;

    /// <summary>
    /// Gets the pixel increase for bold characters.
    /// </summary>
    public const float BOLDINCREASE = 2.45f * BETTER;

    /// <summary>
    /// Gets the width of the display area (in pixels).
    /// </summary>
    public const float DISPLAYAREAWIDTH = 1200;

    /// <summary>
    /// Gets an approximation of how many pixels are an in an em.
    /// </summary>
    public const float EMSTOPIXELS = 34.7f;

    /// <summary>
    /// Gets the maximum name size allowed for a tag.
    /// </summary>
    public const int MAXTAGNAMESIZE = 13;

    /// <summary>
    /// Gets the ratelimit used for displaying hints.
    /// </summary>
    public static TimeSpan HintRateLimit { get; } = TimeSpan.FromMilliseconds(0.525);

    internal const float BETTER = 0.47945214761f;

    /// <summary>
    /// Gets the default color for hints.
    /// </summary>
    public const string DEFAULTCOLOR = "#FFF";

    /// <summary>
    /// Gets the parser used by all elements.
    /// </summary>
    public static Parser Parser { get; } = new(
        new RichTextTag[]
        {
            new SizeTag(),
        });


    /// <summary>
    /// Gets a list of allowed sizes of color param tags, ignoring the hashtag.
    /// </summary>
    public static ReadOnlyCollection<int> ValidColorSizes { get; } = new(new int[]
    {
        3,
        4,
        6,
        8,
    });

    public static ReadOnlyCollection<string> Colors { get; } = new(new string[]
    {
        "black",
        "blue",
        "green",
        "orange",
        "purple",
        "red",
        "white",
        "yellow",
    });

    public static Parser DefaultParser { get; } = new ParserBuilder()
        .AddTag<SizeTag>()
        .AddTag<LineHeightTag>()
        .AddTag<CloseSizeTag>()
        .Build();

    /// <summary>
    /// Gets a <see cref="ReadOnlyDictionary{char, float}"/> of character sizes.
    /// </summary>
    public static ReadOnlyDictionary<char, float> CharacterLengths { get; } = new(new Dictionary<char, float>()
    {
        { ' ', 8.437330252644164f },
        { '!', 7.8282976111695906f },
        { '"', 9.959914233037804f },
        { '#', 20.179779046095838f },
        { '$', 19.229093857778025f },
        { '%', 25.638788760664795f },
        { '&', 21.34585136101609f },
        { '\'', 5.897218504052912f },
        { '(', 11.066568729245208f },
        { ')', 11.304240003997462f },
        { '*', 14.720764578152139f },
        { '+', 19.58560085936392f },
        { ',', 6.639941237557038f },
        { '-', 9.930205323730949f },
        { '.', 8.288785705951272f },
        { '/', 13.777509083281139f },
        { '0', 19.229093857778025f },
        { '1', 19.229093857778025f },
        { '2', 19.229093857778025f },
        { '3', 19.229093857778025f },
        { '4', 19.229093857778025f },
        { '5', 19.229093857778025f },
        { '6', 19.229093857778025f },
        { '7', 19.229093857778025f },
        { '8', 19.229093857778025f },
        { '9', 19.229093857778025f },
        { ':', 7.286112426075135f },
        { ';', 6.75877687492325f },
        { '<', 17.73621887611901f },
        { '=', 19.199385037879118f },
        { '>', 17.97389015087127f },
        { '?', 15.760576405081713f },
        { '@', 31.684551811400684f },
        { 'A', 21.672649363788054f },
        { 'B', 21.264154237035264f },
        { 'C', 22.534207734579077f },
        { 'D', 22.719890794756495f },
        { 'E', 19.741570256661454f },
        { 'F', 19.518753436612197f },
        { 'G', 23.7225641082412f },
        { 'H', 24.554413569725376f },
        { 'I', 9.232045954216844f },
        { 'J', 19.095403855156416f },
        { 'K', 21.888041333173177f },
        { 'L', 18.278408848375236f },
        { 'M', 30.020852888432344f },
        { 'N', 24.63611544718009f },
        { 'O', 23.484892833488946f },
        { 'P', 21.368135419757962f },
        { 'Q', 23.484892833488946f },
        { 'R', 22.029156275866857f },
        { 'S', 20.55114041287765f },
        { 'T', 20.721964264878178f },
        { 'U', 22.809017522776198f },
        { 'V', 21.397844329064814f },
        { 'W', 31.105228079272624f },
        { 'X', 21.22701572368954f },
        { 'Y', 20.7739572329269f },
        { 'Z', 20.759102778223905f },
        { '[', 8.318494615287868f },
        { '\\', 13.673527900558438f },
        { ']', 8.318494615287868f },
        { '^', 14.438529939390035f },
        { '_', 14.98071991154712f },
        { '`', 9.91535086902795f },
        { 'a', 18.5903523964442f },
        { 'b', 19.22909394718597f },
        { 'c', 17.87733857228658f },
        { 'd', 19.28108216185995f },
        { 'e', 17.929326786861417f },
        { 'f', 11.489923064075736f },
        { 'g', 19.25137325255309f },
        { 'h', 19.043410887107694f },
        { 'i', 7.77630939653527f },
        { 'j', 7.909999488574739f },
        { 'k', 16.99349704941301f },
        { 'l', 7.77630939653527f },
        { 'm', 30.74872116719381f },
        { 'n', 19.05826534181069f },
        { 'o', 19.43705631263137f },
        { 'p', 19.22909394718597f },
        { 'q', 19.347929584611663f },
        { 'r', 11.675601370779267f },
        { 's', 17.57281987478261f },
        { 't', 11.148270606699926f },
        { 'u', 19.05826534181069f },
        { 'v', 16.68898219862446f },
        { 'w', 26.158694674179156f },
        { 'x', 16.874660505327988f },
        { 'y', 16.874660505327988f },
        { 'z', 16.488444683843184f },
        { '{', 11.452784550730012f },
        { '|', 7.6574737591789725f },
        { '}', 11.452784550730012f },
        { '~', 23.75227301754805f },
        { '…', 22.244548245251984f },
    });
}