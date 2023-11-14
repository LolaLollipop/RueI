namespace RueI
{
    using System.Collections.ObjectModel;
    using System.Text;

    using NorthwoodLib.Pools;

    using RueI.Enums;
    using RueI.Parsing;
    using RueI.Parsing.Tags;
    using RueI.Records;

    /// <summary>
    /// Helps parse the content of elements.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="tags">The list of tags to initialize with.</param>
        internal Parser(IEnumerable<RichTextTag> tags)
        {
            Tags = new(ExtractTagsToPairs(tags));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="tags">The list of tags to initialize with.</param>
        internal Parser(params RichTextTag[] tags)
        {
            Tags = new(ExtractTagsToPairs(tags));
        }

        /// <summary>
        /// Gets the current tags of the parser.
        /// </summary>
        public ReadOnlyDictionary<string, RichTextTag> Tags { get; }

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

            ParamsTagBase? currentTag = null;
            char? delimiter = null;

            StringBuilder paramBuffer = StringBuilderPool.Shared.Rent(30);

            using ParserContext context = new();

            void FailTagMatch() // not a tag, unload buffer
            {
                this.AvoidMatch(context);
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

            foreach (char ch in text)
            {
                if (ch == '<')
                {
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
                    if ((ch > '\u0060' && ch < '\u007B') || ch == '-') // descend deeper into node
                    {
                        if (tagBufferSize > Constants.MAXTAGNAMESIZE)
                        {
                            FailTagMatch();
                        }

                        tagBuffer.Append(ch);
                        continue;
                    }
                    else if (ch == '>')
                    {
                        if (Tags.TryGetValue(tagBuffer.ToString(), out RichTextTag tag) && tag is NoParamsTagBase noParams)
                        {
                            noParams.HandleTag(context);
                            continue;
                        }
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else if (ch == ' ' || ch == '=')
                    {
                        if (Tags.TryGetValue(tagBuffer.ToString(), out RichTextTag tag) && tag is ParamsTagBase withParams && withParams.IsValidDelimiter(ch))
                        {
                            currentTag = withParams;
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
                else if (currentState == ParserState.CollectingParams)
                {
                    if (ch == '>')
                    {
#pragma warning disable
                        if (currentTag.HandleTag(context, delimiter.Value, paramBuffer.ToString()))
#pragma warning restore
                        {
                            tagBuffer.Clear();
                            paramBuffer.Clear();

                            currentTag = null;
                            delimiter = null;
                            currentState = ParserState.CollectingTags;
                            tagBufferSize = 0;
                        } else
                        {
                            FailTagMatch();
                        }
                    }

                    paramBuffer.Append(ch);
                    continue; // do NOT add as a character
                }

                AddCharacter(context, ch);
            } // foreach

            StringBuilderPool.Shared.Return(tagBuffer);
            StringBuilderPool.Shared.Return(paramBuffer);
            return new ParsedData(context.ResultBuilder.ToString(), context.NewOffset);
        }

        /// <summary>
        /// Adds a character to a parser context.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        /// <param name="ch">The character to add.</param>
        public void AddCharacter(ParserContext context, char ch)
        {
            float size = CalculateCharacterLength(context, ch);

            context.WidthSinceSpace += size;

            context.ResultBuilder.Append(ch);

            if (context.CurrentLineWidth + context.WidthSinceSpace > Constants.DISPLAYAREAWIDTH)
            {
                CreateLineBreak(context);
            }
            else if (ch == ' ')
            {
                if (!context.NoBreak)
                {
                    context.WidthSinceSpace = 0;
                }
            }
        }

        /// <summary>
        /// Calculates the length of an <see cref="char"/> with a context.
        /// </summary>
        /// <param name="context">The context to parse the char under.</param>
        /// <param name="ch">The char to calculate the length for.</param>
        /// <returns>A float indicating the total length of the char.</returns>
        public float CalculateCharacterLength(TextInfo context, char ch)
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

            if (Constants.CharacterLengths.TryGetValue(functionalCase, out float chSize))
            {
                float multiplier = context.Size / 35;
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
        public void CreateLineBreak(ParserContext context)
        {
            if (context.CurrentLineWidth < Constants.DISPLAYAREAWIDTH)
            {
                context.CurrentLineWidth = context.WidthSinceSpace;
                context.NewOffset += context.CurrentLineHeight;
            }
            else
            {
                context.CurrentLineWidth = 0;
                context.NewOffset += context.WidthSinceSpace;
            }
        }

        /// <summary>
        /// Parses the tag attributes of a string.
        /// </summary>
        /// <param name="content">The content to parse.</param>
        /// <param name="attributes">The pairs of attributes.</param>
        /// <returns><see cref="true"/> if the content is valid, otherwise <see cref="false"/>.</returns>
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
        /// Exports this parser's <see cref="RichTextTag"/>s to a <see cref="ParserBuilder"/>.
        /// </summary>
        /// <param name="builder">The builder to export the tags to.</param>
        internal void ExportTo(ParserBuilder builder) => builder.AddTags(Tags.Values);

        /// <summary>
        /// Avoids the client TMP matching a tag.
        /// </summary>
        /// <param name="context">The context of the parser.</param>
        private void AvoidMatch(ParserContext context)
        {
            if (!context.IsMonospace && context.CurrentCSpace == 0)
            {
                context.ResultBuilder.Append('​'); // zero width space
            }
            else if (context.IsBold)
            {
                context.ResultBuilder.Append("<b>");
            } else
            {
                context.ResultBuilder.Append("</b>");
            }
        }

        /// <summary>
        /// Extracts the tags of an <see cref="IEnumerable{T}"/> containing <see cref="RichTextTag"/>s.
        /// </summary>
        /// <param name="tags">The <see cref="IEnumerable{T}"/> containing <see cref="RichTextTag"/>s.</param>
        /// <returns>A new dictionary containing the modified values.</returns>
        private Dictionary<string, RichTextTag> ExtractTagsToPairs(IEnumerable<RichTextTag> tags)
        {
            Dictionary<string, RichTextTag> dict = new();
            foreach (var tag in tags)
            {
                foreach (string name in tag.Names)
                {
                    dict.Add(name, tag);
                }
            }

            return dict;
        }
    }
}
