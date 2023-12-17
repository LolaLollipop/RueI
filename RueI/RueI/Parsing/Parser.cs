namespace RueI.Parsing;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NorthwoodLib.Pools;
using RueI.Elements.Enums;
using RueI.Extensions;
using RueI.Parsing.Enums;
using RueI.Parsing.Records;
using RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Helps parse the content of elements.
/// </summary>
/// <include file='docs.xml' path='docs/members[@name="parser"]/Parser/*'/>
public class Parser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="tags">The list of tags to initialize with.</param>
    /// <param name="backups">The list of parsers to use as a backup.</param>
    internal Parser(IEnumerable<RichTextTag> tags, IEnumerable<Parser> backups)
    {
        IEnumerable<ValueTuple<string, RichTextTag>> tuplePairs = tags.SelectMany(x => x.Names.Select(y => (y, x)));
        Tags = (Lookup<string, RichTextTag>)tuplePairs.ToLookup(x => x.Item1, x => x.Item2);
        TagBackups = new(backups.ToList());
    }

    /// <summary>
    /// Gets the default <see cref="Parser"/>.
    /// </summary>
    public static Parser DefaultParser { get; } = new ParserBuilder().AddFromAssembly(typeof(Parser).Assembly).Build();

    /// <summary>
    /// Gets the tags that will be searched for when parsing.
    /// </summary>
    /// <remarks>
    /// Multiple tags can share the same name.
    /// </remarks>
    public Lookup<string, RichTextTag> Tags { get; }

    /// <summary>
    /// Gets a list of <see cref="Parser"/>s which this <see cref="Parser"/> will include the tags for.
    /// </summary>
    public ReadOnlyCollection<Parser> TagBackups { get; }

    /// <summary>
    /// Adds a character to a parser context.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    /// <param name="ch">The character to add.</param>
    public static void AddCharacter(ParserContext context, char ch)
    {
        float size = CalculateCharacterLength(context, ch);

        // TODO: any chars
        if (ch == ' ' || ch == '​') // zero width space
        {
            context.CurrentLineWidth += size;

            if (!context.NoBreak)
            {
                context.CurrentLineWidth += context.WidthSinceSpace;
                context.WidthSinceSpace = 0;
            }
        }
        else
        {
            if (context.CurrentLineWidth + context.WidthSinceSpace > context.FunctionalWidth)
            {
                CreateLineBreak(context);
            }

            context.WidthSinceSpace += size;
            context.LineHasAnyChars = true;
            if (context.Size > context.BiggestCharSize)
            {
                context.BiggestCharSize = context.Size;
            }
        }
    }

    /// <summary>
    /// Calculates the length of an <see cref="char"/> with a context.
    /// </summary>
    /// <param name="context">The context to parse the char under.</param>
    /// <param name="ch">The char to calculate the length for.</param>
    /// <returns>A float indicating the total length of the char.</returns>
    public static float CalculateCharacterLength(TextInfo context, char ch)
    {
        char functionalCase = context.CurrentCase switch
        {
            CaseStyle.Smallcaps or CaseStyle.Uppercase => char.ToUpper(ch),
            CaseStyle.Lowercase => char.ToLower(ch),
            _ => ch
        };

        if (context.IsMonospace)
        {
            return context.Monospacing + context.CurrentCSpace;
        }

        if (CharacterLengths.Lengths.TryGetValue(functionalCase, out float chSize))
        {
            float multiplier = context.Size / Constants.DEFAULTSIZE;
            if (context.CurrentCase == CaseStyle.Smallcaps && char.IsLower(ch))
            {
                multiplier *= 0.8f;
            }

            if (context.IsSuperOrSubScript)
            {
                multiplier *= 0.5f;
            }

            return chSize * multiplier;
        }
        else
        {
            // TODO: handle warnings
            return default;
        }
    }

    /// <summary>
    /// Generates the effects of a linebreak for a parser.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    /// <param name="isOverflow">Whether or not the line break was caused by an overflow.</param>
    public static void CreateLineBreak(ParserContext context, bool isOverflow = false)
    {
        if (context.LineHasAnyChars)
        {
            context.NewOffset += CalculateSizeOffset(context.BiggestCharSize);
        }
        else
        {
            context.NewOffset += CalculateSizeOffset(Constants.DEFAULTSIZE);
        }

        if (context.WidthSinceSpace > context.FunctionalWidth)
        {
            context.CurrentLineWidth = 0;
        }
        else
        {
            context.CurrentLineWidth = context.WidthSinceSpace;
        }

        context.BiggestCharSize = 0;
        context.LineHasAnyChars = false;
        context.NewOffset += context.CurrentLineHeight; // TODO: support margins

        if (!isOverflow)
        {
            context.CurrentLineWidth += context.LineIndent;
            context.WidthSinceSpace = 0f;
        }
    }

    /// <summary>
    /// Parses the tag attributes of a string.
    /// </summary>
    /// <param name="content">The content to parse.</param>
    /// <param name="attributes">The pairs of attributes.</param>
    /// <returns>true if the content is valid, otherwise false.</returns>,
    public static bool GetTagAttributes(string content, out Dictionary<string, string> attributes)
    {
        IEnumerable<string> result = content.Split('"')
                        .Select((element, index) => index % 2 == 0
                           ? element.Split(' ')
                           : new string[] { element })
                        .SelectMany(element => element);

        Dictionary<string, string> attributePairs = new();
        attributes = attributePairs;

        foreach (string possiblePair in result)
        {
            if (possiblePair == string.Empty)
            {
                return false;
            }

            string[] results = possiblePair.Split('=');

            if (results.Length != 2)
            {
                return false;
            }

            attributePairs.Add(results[0], results[1]);
        }

        return true;
    }

    /// <summary>
    /// Parses a rich text string.
    /// </summary>
    /// <param name="text">The string to parse.</param>
    /// <returns>A <see cref="ParsedData"/> containing information about the string.</returns>
    public ParsedData Parse(string text)
    {
        ParserState currentState = ParserState.CollectingTags;

        StringBuilder tagBuffer = StringBuilderPool.Shared.Rent(Constants.MAXTAGNAMESIZE);
        int tagBufferSize = 0;

        RichTextTag? currentTag = null;
        char? delimiter = null;

        StringBuilder paramBuffer = StringBuilderPool.Shared.Rent(30);

        using ParserContext context = new();

        void FailTagMatch() // not a tag, unload buffer
        {
            AddCharacter(context, '<');

            AvoidMatch(context);
            foreach (char ch in tagBuffer.ToString())
            {
                AddCharacter(context, ch);
            }

            foreach (char ch in paramBuffer.ToString())
            {
                AddCharacter(context, ch);
            }

            if (delimiter != null)
            {
                AddCharacter(context, delimiter.Value);
                delimiter = null;
            }

            tagBuffer.Clear();
            paramBuffer.Clear();

            currentTag = null;
            currentState = ParserState.CollectingTags;
            tagBufferSize = 0;
        }

        char[] chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char ch = chars[i];

            if (ch == '\\')
            {
                int original = i;
                int length = chars.Length;
                for (; i < length && chars[i + 1] == '\\'; i++)
                {
                    context.ResultBuilder.Append(chars[i]);
                }

                int matcher = i - original;
                int times = matcher - (int)Math.Floor(matcher / 3d);

                if ((i - original) % 3 == 0)
                {
                    switch (chars[i])
                    {
                        case 'n':
                            CreateLineBreak(context);
                            i++;
                            break;
                        case 'r':
                            context.CurrentLineWidth = 0;
                            i++;
                            break;
                        case 'u':
                            context.ResultBuilder.Append('\\'); // TODO: add support for unicode literals
                            break;
                        default:
                            break;
                    }
                }

                for (int newIndex = 0; newIndex < times; newIndex++)
                {
                    AddCharacter(context, chars[i + newIndex]);
                }
            }
            else if (ch == '<')
            {
                if (currentState != ParserState.CollectingTags)
                {
                    FailTagMatch();
                }

                currentState = ParserState.DescendingTag;
                continue; // do NOT add as a character
            }
            else if (ch == '\n')
            {
                context.ResultBuilder.Append('\n');
                CreateLineBreak(context);
                if (currentState != ParserState.CollectingTags)
                {
                    FailTagMatch();
                }

                continue; // do NOT add as a character
            }
            else if (currentState == ParserState.DescendingTag)
            {
                if ((ch > '\u0060' && ch < '\u007B') || ch == '-' || ch == '/')
                {
                    if (tagBufferSize > Constants.MAXTAGNAMESIZE)
                    {
                        FailTagMatch();
                    }

                    tagBuffer.Append(ch);
                    continue; // do NOT add as a character
                }
                else if (ch == '>')
                {
                    if (TryGetBestMatch(tagBuffer.ToString(), TagStyle.NoParams, out RichTextTag? tag))
                    {
                        if (context.ShouldParse || tag is CloseNoparse)
                        {
                            tag!.HandleTag(context, string.Empty);
                            continue;
                        }
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else
                    {
                        FailTagMatch();
                    }
                }
                else if (ch == ' ' || ch == '=')
                {
                    if (context.ShouldParse)
                    {
                        TagStyle style = ch switch
                        {
                            ' ' => TagStyle.Attributes,
                            '=' => TagStyle.ValueParam,
                            _ => throw new ArgumentOutOfRangeException(nameof(ch)),
                        };

                        if (TryGetBestMatch(tagBuffer.ToString(), style, out RichTextTag? tag))
                        {
                            currentTag = tag;
                            delimiter = ch;

                            currentState = ParserState.CollectingParams;
                            continue;
                        }
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else
                    {
                        FailTagMatch();
                    }
                }
                else
                {
                    FailTagMatch();
                }
            }
            else if (currentState == ParserState.CollectingParams)
            {
                if (ch == '>')
                {
                    if (currentTag!.HandleTag(context, paramBuffer.ToString()))
                    {
                        tagBuffer.Clear();
                        paramBuffer.Clear();

                        currentTag = null;
                        delimiter = null;
                        currentState = ParserState.CollectingTags;
                        tagBufferSize = 0;
                    }
                    else
                    {
                        FailTagMatch();
                    }
                }

                paramBuffer.Append(ch);
                continue; // do NOT add as a character
            }

            AddCharacter(context, ch);
        } // foreach

        context.ApplyClosingTags();
        if (context.WidthSinceSpace > 0 || context.CurrentLineWidth > 0)
        {
            context.NewOffset += CalculateSizeOffset(context.BiggestCharSize);
        }

        StringBuilderPool.Shared.Return(tagBuffer);
        StringBuilderPool.Shared.Return(paramBuffer);
        return new ParsedData(context.ResultBuilder.ToString(), context.NewOffset);
    }

    /// <summary>
    /// Parses a rich text string.
    /// </summary>
    /// <param name="text">The string to parse.</param>
    /// <param name="options">The options for the elment.</param>
    /// <returns>A <see cref="ParsedData"/> containing information about the string.</returns>
    public ParsedData ParseFast(string text, ElementOptions options = ElementOptions.Default)
    {
        using ParserContext context = new();

        int length = text.Length;
        bool noparseIgnoresEscape = options.HasFlagFast(ElementOptions.NoparseIgnoresEscape);

        for (int i = 0; i < length; i++)
        {
            char ch = text[i];

            if (ch == '\\')
            {
                int original = i;
                for (; i < length && text[i + 1] == '\\'; i++)
                {
                    context.ResultBuilder.Append(text[i]);
                }

                int matcher = i - original;
                int times = matcher - (int)Math.Floor(matcher / 3d);

                if ((i - original) % 3 == 0)
                {
                    if (context.ShouldParse || noparseIgnoresEscape)
                    {
                        switch (text[i])
                        {
                            case 'n':
                                CreateLineBreak(context);
                                i++;
                                break;
                            case 'r':
                                context.CurrentLineWidth = 0;
                                i++;
                                break;
                            case 'u':
                                context.ResultBuilder.Append('\\'); // TODO: add support for unicode literals
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        context.ResultBuilder.Append('\\');
                    }
                }

                for (int newIndex = 0; newIndex < times; newIndex++)
                {
                    char newCh = text[i + newIndex];
                    AddCharacter(context, newCh);
                }
            }
            else if (ch == '<')
            {
                HandleTagFast(context, text, ref i);
                continue; // do NOT add as a character
            }
            else if (ch == '\n')
            {
                context.ResultBuilder.Append('\n');
                CreateLineBreak(context);

                continue; // do NOT add as a character
            }

            AddCharacter(context, ch);
        } // foreach

        context.ApplyClosingTags();
        if (context.WidthSinceSpace > 0 || context.CurrentLineWidth > 0)
        {
            context.NewOffset += CalculateSizeOffset(context.BiggestCharSize);
        }

        return new ParsedData(context.ResultBuilder.ToString(), context.NewOffset);
    }

    private void HandleTagFast(ParserContext context, string text, ref int index)
    {
        int length = text.Length;
        if ((index + 1) >= length)
        {
            return;
        }

        int start = index + 1;
        char ch = text[start];
        while ((index + 2) < length)
        {
            if (!IsValidTagChar(ch) || (index - start) == Constants.MAXTAGNAMESIZE)
            {
                break;
            }

            index++;
            ch = text[index + 1];
        }

        if (ch == '>')
        {
            string name = text.Substring(start, index - start + 1);
            if (TryGetBestMatch(name, TagStyle.NoParams, out RichTextTag? tag))
            {
                if (context.ShouldParse || tag is CloseNoparse)
                {
                    tag!.HandleTag(context, string.Empty);
                    index++;
                    return;
                }
            }
        }
        else if (context.ShouldParse)
        {
            RichTextTag? tag = ch switch
            {
                ' ' => GetBestMatch(text.Substring(start, index - start + 1), TagStyle.Attributes),
                '=' => GetBestMatch(text.Substring(start, index - start + 1), TagStyle.ValueParam),
                _ => null
            };

            if (tag != null)
            {
                index += 2;
                int delimiter = index;

                while ((index + 2) < length)
                {
                    if (ch == '>' || (index - start) > 127)
                    {
                        break;
                    }

                    index++;
                    ch = text[index + 1];
                }

                if (((index - start) < 127) && (index + 2) < length)
                {
                    string parameters = text.Substring(delimiter, index - delimiter + 1);
                    bool wasSuccessful = tag.HandleTag(context, parameters);

                    if (wasSuccessful)
                    {
                        index++;
                        return;
                    }
                }

                index -= 2;
            }
        }

        context.ResultBuilder.Append('<');
        AddCharacter(context, '<');
        AvoidMatch(context);
        for (int i = start; i < (index + 1); i++)
        {
            char current = text[i];
            context.ResultBuilder.Append(current);
            AddCharacter(context, current);
        }
    }

    private static void FailBuffer(ParserContext context, string buffer)
    {
        foreach (char addChar in buffer)
        {
            AddCharacter(context, addChar);
            context.ResultBuilder.Append(addChar);
        }
    }

    private static float CalculateSizeOffset(float biggestChar) => (((biggestChar / Constants.DEFAULTSIZE * 0.2f) + 0.8f) * Constants.DEFAULTHEIGHT) - Constants.DEFAULTHEIGHT;

    private static bool IsValidTagChar(char ch) => (ch > '\u0060' && ch < '\u007B') || ch == '-' || ch == '/';

    /// <summary>
    /// Avoids the client TMP matching a tag.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    private static void AvoidMatch(ParserContext context)
    {
        if (!context.IsMonospace && context.CurrentCSpace == 0)
        {
            context.ResultBuilder.Append('​'); // zero width space
        }
        else if (context.IsBold)
        {
            context.ResultBuilder.Append("<b>");
        }
        else
        {
            context.ResultBuilder.Append("</b>");
        }
    }

    private bool TryGetBestMatch(string name, TagStyle style, out RichTextTag? tag)
    {
        tag = null;

        RichTextTag? chosenTag = Tags[name].FirstOrDefault(x => x.TagStyle == style);
        if (chosenTag != null)
        {
            tag = chosenTag;
            return true;
        }

        return false;
    }

    private RichTextTag? GetBestMatch(string name, TagStyle style)
    {
        foreach (Parser parser in TagBackups)
        {
            RichTextTag? tag = Tags[name].FirstOrDefault(x => x.TagStyle == style);
            if (tag != null)
            {
                return tag;
            }
        }

        return Tags[name].FirstOrDefault(x => x.TagStyle == style) ?? GetTagBackups(name, style);
    }

    private RichTextTag? GetTagBackups(string name, TagStyle style)
    {
        foreach (Parser parser in TagBackups)
        {
            RichTextTag? tag = Tags[name].FirstOrDefault(x => x.TagStyle == style);
            if (tag != null)
            {
                return tag;
            }
        }

        return null;
    }
}
