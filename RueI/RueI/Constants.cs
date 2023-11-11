namespace RueI
{
    using System.Collections.ObjectModel;
    using RueI.Parsing;
    using RueI.Parsing.Tags.ConcreteTags;

    /// <summary>
    /// Provides a variety of constant values.
    /// </summary>
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
        /// Gets the pixel amount applied to turn something into a pixel.
        /// </summary>
        public const float BOLDINCREASE = 2.45f * BETTER;

        /// <summary>
        /// Gets the width of the display area (in pixels).
        /// </summary>
        public const float DISPLAYAREAWIDTH = 1200; // technically 1200, decreased slightly

        /// <summary>
        /// Gets an approximation of how many pixels are an in an em.
        /// </summary>
        public const float EMSTOPIXELS = 34.7f;

        /// <summary>
        /// Gets the maximum name size allowed for a tag.
        /// </summary>
        public const int MAXTAGNAMESIZE = 13;

        /// <summary>
        /// Gets a string used to close all tags.
        /// </summary>
        public const string TAGCLOSER = "</noparse></align></color></b></i></cspace></line-height></line-indent></link></lowercase></uppercase></smallcaps></margin></mark></mspace></pos></size></s></u></voffset></width>";

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

        /// <summary>
        /// Gets a <see cref="ReadOnlyDictionary{char, float}"/> of character sizes.
        /// </summary>
        public static ReadOnlyDictionary<char, float> OldCharacterLengths { get; } = new(new Dictionary<char, float>()
        {
            { ' ', 8.510275620072498f },
            { '!', 7.895977555949061f },
            { '"', 10.046023177636238f },
            { '#', 20.354244351241235f },
            { '$', 19.39534005600124f },
            { '%', 25.860450211688125f },
            { '&', 21.530398003612184f },
            { '\'', 5.948203206287188f },
            { '(', 11.162245311523439f },
            { ')', 11.401971385358438f },
            { '*', 14.848033696324062f },
            { '+', 19.754929166803738f },
            { ',', 6.697347186924062f },
            { '-', 10.016057418444362f },
            { '.', 8.360446823953124f },
            { '/', 13.896623238142174f },
            { '0', 19.39534005600124f },
            { '1', 19.39534005600124f },
            { '2', 19.39534005600124f },
            { '3', 19.39534005600124f },
            { '4', 19.39534005600124f },
            { '5', 19.39534005600124f },
            { '6', 19.39534005600124f },
            { '7', 19.39534005600124f },
            { '8', 19.39534005600124f },
            { '9', 19.39534005600124f },
            { ':', 7.349104847347488f },
            { ';', 6.8172102238315615f },
            { '<', 17.889558257649373f },
            { '=', 19.365374296809364f },
            { '>', 18.129284331484378f },
            { '?', 15.896835269239688f },
            { '@', 31.958482214084686f },
            { 'A', 21.860021355122814f },
            { 'B', 21.447994562994676f },
            { 'C', 22.729028372587187f },
            { 'D', 22.91631676509655f },
            { 'E', 19.912247005450936f },
            { 'F', 19.687503811261873f },
            { 'G', 23.92765874166219f },
            { 'H', 24.76669999993469f },
            { 'I', 9.3118620766253f },
            { 'J', 19.2604941394878f },
            { 'K', 22.07727550672405f },
            { 'L', 18.436435760861233f },
            { 'M', 30.28039969753969f },
            { 'N', 24.849108235122486f },
            { 'O', 23.687932667827187f },
            { 'P', 21.552874720316236f },
            { 'Q', 23.687932667827187f },
            { 'R', 22.219610465825312f },
            { 'S', 20.728816341589678f },
            { 'T', 20.90111705988281f },
            { 'U', 23.006214042772175f },
            { 'V', 21.58284047950811f },
            { 'W', 31.374149909193125f },
            { 'X', 21.410534966744688f },
            { 'Y', 20.953559535778737f },
            { 'Z', 20.938576656132803f },
            { '[', 8.390412583175f },
            { '\\', 13.791743080820611f },
            { ']', 8.390412583175f },
            { '^', 14.56335898365125f },
            { '_', 15.110236486763112f },
            { '`', 10.001074538798424f },
            { 'a', 18.751076232725925f },
            { 'b', 19.39534005600124f },
            { 'c', 18.031898011320923f },
            { 'd', 19.447777737426875f },
            { 'e', 18.08433569264656f },
            { 'f', 11.589259777767799f },
            { 'g', 19.417811978234997f },
            { 'h', 19.208051663591874f },
            { 'i', 7.843539874563424f },
            { 'j', 7.978385791086863f },
            { 'k', 17.1404142770525f },
            { 'l', 7.843539874563424f },
            { 'm', 31.01456079849062f },
            { 'n', 19.22303454323781f },
            { 'o', 19.60510037064436f },
            { 'p', 19.39534005600124f },
            { 'q', 19.515203092968733f },
            { 'r', 11.776543375706874f },
            { 's', 17.724746581944064f },
            { 't', 11.244653546711238f },
            { 'u', 19.22303454323781f },
            { 'v', 16.833267642245925f },
            { 'w', 26.38485099819594f },
            { 'x', 17.020551240185f },
            { 'y', 17.020551240185f },
            { 'z', 16.630996370190626f },
            { '{', 11.551800181517812f },
            { '|', 7.723676837665924f },
            { '}', 11.551800181517812f },
            { '~', 23.95762450085406f },
            { '…', 22.436864617426547f },
        });

    }
}