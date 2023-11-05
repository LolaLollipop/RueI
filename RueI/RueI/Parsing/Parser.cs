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
        public Parser(IEnumerable<RichTextTag> tags)
        {
            Tags = new(ExtractTagsToPairs(tags));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="tags">The list of tags to initialize with.</param>
        public Parser(params RichTextTag[] tags)
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

            ParamProcessor? paramProcessor = null;

            using ParserContext context = new();
            context.ResultBuilder.Append("<line-height=")
                                 .Append(Constants.DEFAULTHEIGHT)
                                 .Append('>');

            void FailTagMatch() // not a tag, unload buffer
            {
                context.ResultBuilder.Append("<​"); // zero width space guarantees that the tag isnt matched
                foreach (char ch in tagBuffer.ToString())
                {
                    AddCharacter(context, ch);
                }

                tagBuffer.Clear();

                currentState = ParserState.CollectingTags;
                tagBufferSize = 0;
            }

            void FailTagMatchParams(string paramBuffer)
            {
                FailTagMatch();

                foreach (char ch in paramBuffer)
                {
                    AddCharacter(context, ch);
                }
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
                    else if (ch == '=')
                    {
                        if (Tags.TryGetValue(tagBuffer.ToString(), out RichTextTag tag) && tag.TryGetNewProcessor(out ParamProcessor? processor))
                        {
                            paramProcessor = processor;

                            tagBuffer.Append('=');
                            continue; // do NOT add as a character
                        }
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else if (ch == '>')
                    {
                        if (Tags.TryGetValue(tagBuffer.ToString(), out RichTextTag tag))
                        {
                            if (tag is NoParamsTagBase noParams)
                            {
                                noParams.HandleTag(context);

                                tagBuffer.Clear();

                                paramProcessor?.Dispose();
                                paramProcessor = null;
                                continue; // do NOT add as a character
                            }
                            else if (currentState == ParserState.CollectingParams && paramProcessor != null)
                            {
                                bool wasSuccessful = paramProcessor.GetFinishResult(context, out string? unloaded);
                                if (!wasSuccessful)
                                {
                                    FailTagMatchParams(unloaded!);
                                }
                                else
                                {
                                    paramProcessor?.Dispose();
                                    paramProcessor = null;
                                    tagBuffer.Clear();

                                    continue;
                                }
                            }
                            else
                            {
                                FailTagMatch();
                            }

                            paramProcessor?.Dispose();
                            paramProcessor = null;
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
                    paramProcessor?.Add(ch);
                    continue; // do NOT add as a character
                }

                AddCharacter(context, ch);
            } // foreach

            paramProcessor?.Dispose();
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
        public float CalculateCharacterLength(ParserContext context, char ch)
        {
            char functionalCase = context.CurrentCase switch
            {
                CaseStyle.Smallcaps or CaseStyle.Uppercase => char.ToUpper(ch),
                CaseStyle.Lowercase => char.ToLower(ch),
                _ => ch
            };

            if (context.IsMonospace)
            {
                return context.CurrentCSpace;
            }

            if (Constants.CharacterLengths.TryGetValue(functionalCase, out float chSize))
            {
                float multiplier = context.Size / 35;
                if (context.CurrentCase == CaseStyle.Smallcaps && char.IsLower(ch))
                {
                    multiplier *= 0.8f;
                }

                return (chSize * multiplier) + chSize;
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
