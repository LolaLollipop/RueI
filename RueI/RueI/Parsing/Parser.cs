namespace RueI
{
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.Remoting.Contexts;
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
        private readonly ParserNode baseNodes = new();
        private readonly List<RichTextTag> tags = new();

        /// <summary>
        /// Gets the current tags of the parser.
        /// </summary>
        public ReadOnlyCollection<RichTextTag> CurrentTags => tags.AsReadOnly();

        /// <summary>
        /// Adds a tag to the parser.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        public void AddTag(RichTextTag tag)
        {
            tags.Add(tag);
            Reassemble();
        }

        /// <summary>
        /// Parses a rich text string.
        /// </summary>
        /// <param name="text">The string to parse.</param>
        /// <returns>A <see cref="ParsedData"/> containing information about the string.</returns>
        public ParsedData Parse(string text)
        {
            ReadOnlyDictionary<char, float> charSizes = Constants.CharacterLengths;

            ParserState currentState = ParserState.CollectingTags;
            ParserNode? currentNode = null;
            StringBuilder tagBuffer = StringBuilderPool.Shared.Rent();

            ParamProcessor? paramProcessor = null;

            using ParserContext context = new();
            void FailTagMatch() // not a tag, unload buffer
            {
                context.ResultBuilder.Append("<​"); // zero width space guarantees that the tag isnt matched
                foreach (char ch in tagBuffer.ToString())
                {
                    AddCharacter(context, ch);
                }

                tagBuffer.Clear();

                currentState = ParserState.CollectingTags;
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
                    currentNode = baseNodes;
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
                        if (currentNode?.Branches?.TryGetValue(ch, out ParserNode node) == true)
                        {
                            currentNode = node;
                            tagBuffer.Append(ch);
                            continue; // do NOT add as a character
                        }
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else if (ch == '=')
                    {
                        if (currentNode?.Tag != null)
                        {
                            if (ch == '=' && currentNode.Tag.TryGetNewProcessor(out ParamProcessor? processor))
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
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else if (ch == '>')
                    {
                        if (currentNode?.Tag != null)
                        {
                            if (currentNode.Tag is NoParamsTagBase noParams)
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
                                } else
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

        public void AddCharacter(ParserContext context, char ch)
        {
            float size = CalculateCharacterLength(context, ch);

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
        /// <param name="ch">The char to calculate the length for.</param>
        /// <param name="context">The context to parse the string under.</param>
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
            }
        }

        private void Reassemble()
        {
            foreach (RichTextTag tag in tags)
            {
                foreach (string name in tag.Names)
                {
                    ParserNode currentNode = baseNodes;

                    foreach (char ch in name)
                    {
                        if (!currentNode.Branches.TryGetValue(ch, out ParserNode node))
                        {
                            node = new ParserNode();
                            currentNode.Branches.Add(ch, node);
                        }

                        currentNode = node;
                    }

                    currentNode.Tag = tag;
                }
            }
        }
    }
}
