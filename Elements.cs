namespace RueI
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines a record that contains information used for displaying multiple elements.
    /// </summary>
    /// <param name="Content">The element's content.</param>
    /// <param name="Offset">The offset that should be applied. Equivalent to the total linebreaks within the element.</param>
    public record struct ParsedData(string Content, float Offset);

    /// <summary>
    /// Represents an element that is shown when certain screens are active.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier</typeparam>
    public interface IScreenElement<T>
        where T: Enum
    {
        /// <summary>
        /// Gets or sets the screens that this element is shown on.
        /// </summary>
        public T Screens { get; set; }
    }

    /// <summary>
    /// Represents a wrapper for an element that provides screen functionality.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    /// <typeparam name="U">The type of the element to wrap around.</typeparam>
    public class ScreenEle<T, U> : Element, IScreenElement<T>
        where T : Enum
        where U : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenEle{T, U}"/> class.
        /// </summary>
        /// <param name="screens">The screens that this element is visible on.</param>
        /// <param name="element">The element to be used as the base.</param>
        public ScreenEle(T screens, U element)
            : base(element.Position, element.ZIndex)
        {
            Inner = element;
            Screens = screens;
        }

        /// <inheritdoc/>
        public T Screens { get; set; }

        /// <summary>
        /// Gets the element that this is wrapping around.
        /// </summary>
        public U Inner { get; }

        /// <inheritdoc/>
        public override ParsedData ParsedData => Inner.ParsedData;
    }

    /// <summary>
    /// Represents a <see cref="DynamicElement"/> that is tied to a number of screens.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    public class ScreenSetElement<T> : SetElement, IScreenElement<T>
        where T : Enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenSetElement{T}"/> class.
        /// </summary>
        /// <param name="screens">The screens that this element is visible on.</param>
        /// <param name="position">The position of the element, where 0 is the hint baseline (roughly middle-bottom of the screen).</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        /// <param name="content">The content to set the element to.</param>
        public ScreenSetElement(T screens, float position, int zIndex = 0, string content = "")
            : base(position, zIndex, content)
        {
            Screens = screens;
        }

        /// <inheritdoc/>
        public T Screens { get; set; }
    }

    /// <summary>
    /// Represents a <see cref="DynamicElement"/> that it is tied to a number of screens.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    public class ScreenDynamicElement<T> : DynamicElement, IScreenElement<T>
        where T : Enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenDynamicElement{T}"/> class.
        /// </summary>
        /// <param name="contentGetter">A delegate returning the new content that will be ran every time the display is updated.</param>
        /// <param name="screens">The screens that this element is visible on.</param>
        /// <param name="position">The position of the element, where 0 is the hint baseline (roughly middle-bottom of the screen).</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        public ScreenDynamicElement(GetContent contentGetter, T screens, float position, int zIndex = 0)
            : base(contentGetter, position, zIndex)
        {
            Screens = screens;
        }

        /// <inheritdoc/>
        public T Screens { get; set; }
    }

    /// <summary>
    /// Represents a non-cached element that evaluates and parses a function when getting its content.
    /// </summary>
    public class DynamicElement : Element
    {
        /// <summary>
        /// Defines a method used to get content for an element.
        /// </summary>
        /// <returns>A string with the new content.</returns>
        public delegate string GetContent();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicElement"/> class.
        /// </summary>
        /// <param name="contentGetter">A delegate returning the new content that will be ran every time the display is updated.</param>
        /// <param name="position">The position of the element, where 0 is the hint baseline (roughly middle-bottom of the screen).</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        public DynamicElement(GetContent contentGetter, float position, int zIndex = 0)
            : base(position, zIndex)
        {
            ContentGetter = contentGetter;
        }

        /// <summary>
        /// Gets or sets a method that returns the new content and is called every time the display is updated.
        /// </summary>
        public GetContent ContentGetter { get; set; }

        /// <inheritdoc/>
        public override ParsedData ParsedData => Parse(ContentGetter());
    }

    /// <summary>
    /// Represents a simple cached element with settable content.
    /// </summary>
    public class SetElement : Element
    {
        private ParsedData cachedParsedData;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetElement"/> class.
        /// </summary>
        /// <param name="position">The position of the element, where 0 is the hint baseline (roughly middle-bottom of the screen).</param>
        /// <param name="zIndex">A value determing the priority of the hint, where higher numbers means that it will render above hints with a lower number.</param>
        /// <param name="content">The content to set the element to.</param>
        public SetElement(float position, int zIndex = 0, string content = "") : base(position, zIndex)
        {
            Position = position;
            Content = content;
            cachedParsedData = Parse(content);
        }

        /// <inheritdoc/>
        public override ParsedData ParsedData => cachedParsedData;

        /// <summary>
        /// Gets the raw content of the element.
        /// </summary>
        public virtual string Content { get; protected set; }

        /// <summary>
        /// Sets the content of this element.
        /// </summary>
        /// <param name="content">The text to set the content to (will be parsed)</param>
        public virtual void Set(string content)
        {
            Content = content;
            cachedParsedData = Parse(content);
        }
    }

    /// <summary>
    /// Represents the base class for all elements.
    /// </summary>
    public abstract class Element : IComparable<Element>
    {
        // match line heights, line height closers, noparse tags, noparse closing tags, br, and newlines
        protected static readonly Regex ParserRegex = new(@"<(?:line-height=(-?[0-9]\d*(?:\.\d+?)?)(px|em|ems|%)>|/line-height>|noparse>|/noparse>|br>)|\n");

        public Element(float position, int zIndex) {
            Position = position;
            ZIndex = zIndex;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this element is enabled and will show.
        /// </summary>
        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets the data used for parsing.
        /// </summary>
        public abstract ParsedData ParsedData { get; }

        /// <summary>
        /// Gets or sets the position of the element, in pixels, relative to the baseline.
        /// </summary>
        public virtual float Position { get; set; }

        /// <summary>
        /// Gets or sets the priority of the hint (determining if it shows above another hint).
        /// </summary>
        public virtual int ZIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value that is added to the total linebreaks when calculating the offset.
        /// </summary>
        /// <remarks>Use this for if you know that there will be additional linebreaks caused by overflows.</remarks>
        public virtual int AdditionalLineBreaks { get; set; } = 0;

        /// <summary>
        /// Calculates the offset for two hints.
        /// </summary>
        /// <param name="hintOnePos">The first hint's vertical position.</param>
        /// <param name="hintOneTotalLines">The first hint's total line-height, excluding the vertical position.</param>
        /// <param name="hintTwoPos">The second hint's vertical position.</param>
        /// <returns>A float indicating the new offset.</returns>
        public static float CalculateOffset(float hintOnePos, float hintOneTotalLines, float hintTwoPos)
        {
            float calc = (hintOnePos + (2 * hintOneTotalLines)) - hintTwoPos;
            return calc / -2;
        }

        /// <summary>
        /// Compares an element to another element
        /// </summary>
        /// <param name="other">The other element.</param>
        /// <returns>An <see cref="int"/> indicating whether not this element has a larger <see cref="ZIndex"/>.</returns>
        public int CompareTo(Element other) => this.ZIndex - other.ZIndex;

        /// <summary>
        /// Parses an element's content, providing the offset to maintain a constant position
        /// </summary>
        /// <param name="content">The content to parsed.</param>
        /// <returns>A <see cref="ParsedData"/> containing the new content and position</returns>
        protected static ParsedData Parse(string content)
        {
            bool shouldParse = true;
            float currentHeight = PlayerDisplay.DEFAULTHEIGHT; // in pixels
            float newOffset = 0;

            content = $"<line-height={PlayerDisplay.DEFAULTHEIGHT}px>" + content;
            string newString = ParserRegex.Replace(content, (match) =>
            {
                string small = match.Value.Substring(0, Math.Min(5, match.Value.Length));

                switch (small)
                {
                    case "<line" when shouldParse:

                        if (!float.TryParse(match.Groups[1].ToString(), out float value))
                        {
                            currentHeight = PlayerDisplay.DEFAULTHEIGHT;
                            return $"<line-height={currentHeight}px>";
                        }

                        switch (match.Groups[2].ToString())
                        {
                            case "%":
                                currentHeight = (value / 100) * PlayerDisplay.DEFAULTHEIGHT;
                                break;
                            case "em" or "ems":
                                currentHeight = value * PlayerDisplay.EMSTOPIXELS;
                                break;
                        }
                        return $"<line-height={currentHeight}px>";
                    case "</lin" when shouldParse:
                        currentHeight = PlayerDisplay.DEFAULTHEIGHT;
                        return $"<line-height={currentHeight}px>";
                    case "</nop":
                        shouldParse = true;
                        break;
                    case "<nopa":
                        shouldParse = false;
                        break;
                    case "<br>" when shouldParse:
                        newOffset += currentHeight;
                        break;
                    case "\n":
                        newOffset += currentHeight;
                        break;
                }

                return match.ToString();
            });
            return new ParsedData(newString, newOffset);
        }
    }
}