namespace RueI
{
    using System.Text;
    using Hints;
    //  using Exiled.API.Features;
    using MEC;
    using NorthwoodLib.Pools;
    using RueI.Events;
    using RueI.Records;

    /// <summary>
    /// Represents a <see cref="Display"/> that hides elements based on an active screen.
    /// </summary>
    /// <typeparam name="T">The enum to be used as the screen identifier.</typeparam>
    public class ScreenPlayerDisplay<T> : Display
        where T : Enum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenPlayerDisplay{T}"/> class.
        /// </summary>
        /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
        /// <param name="defaultScreen">The default <see cref="T"/> to use as a screen.</param>
        public ScreenPlayerDisplay(ReferenceHub hub, T defaultScreen)
            : base(hub)
        {
            CurrentScreen = defaultScreen;
        }

        /// <summary>
        /// Gets or sets the current screen that the display is on.
        /// </summary>
        /// <remarks>Updating this does not automatically update the display.</remarks>
        public T CurrentScreen { get; set; }

        /// <summary>
        /// Updates the display if the current screen is a certain screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        public void Update(T screen)
        {
            if (CurrentScreen.Equals(screen))
            {
                Update();
            }
        }

        protected override bool ShouldParse(Element element)
        {
            if (!element.Enabled)
            {
                return false;
            }

            if (element is IScreenElement<T> screenElement)
            {
                return screenElement.Screens.HasFlag(CurrentScreen);
            }

            return true;
        }
    }

    /// <summary>
    /// Represents a display attached to a <see cref="DisplayCoordinator"/>.
    /// </summary>
    public class Display
    {
        /// <summary>
        /// Gets the default height if a line-height is not provided.
        /// </summary>
        public const float DEFAULTHEIGHT = 41; // in pixels;

        /// <summary>
        /// Gets an approximation of how many pixels are an in an em.
        /// </summary>
        public const float EMSTOPIXELS = 35;

        /// <summary>
        /// Gets a string used to close all tags.
        /// </summary>
        public const string TAGCLOSER = "</noparse></align></color></b></i></cspace></line-height></line-indent></link></lowercase></uppercase></smallcaps></margin></mark></mspace></pos></size></s></u></voffset></width>";

        /// <summary>
        /// Gets the ratelimit used for displaying hints.
        /// </summary>
        public const float HINTRATELIMIT = 0.55f;

        private CoroutineHandle? rateLimitTask;
        private bool rateLimitActive = false;
        private bool shouldUpdate = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Display"/> class.
        /// </summary>
        /// <param name="hub">The <see cref="ReferenceHub"/> to assign the display to.</param>
        public Display(ReferenceHub hub)
        {
            ReferenceHub = hub;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Display"/> class.
        /// </summary>
        ~Display()
        {
            if (rateLimitTask is CoroutineHandle ch)
            {
                Timing.KillCoroutines(ch);
            }
        }

        /// <summary>
        /// Gets the ReferenceHub that this display is assigned to.
        /// </summary>
        public ReferenceHub ReferenceHub { get; }

        /// <summary>
        /// Gets the elements of this display.
        /// </summary>
        public List<Element> Elements { get; } = new();

        protected static Comparison<Element> Comparer { get; } = (Element first, Element other) => other.ZIndex - first.ZIndex;

        /// <summary>
        /// Updates this display and shows the player the new hint, if the rate limit is not active.
        /// </summary>
        public void Update()
        {
            if (!rateLimitActive)
            {
                rateLimitActive = true;
                Timing.CallDelayed(HINTRATELIMIT, OnRateLimitFinished);

                string text = ParseElements();
                ReferenceHub.hints.Show(new TextHint(text, new HintParameter[] { new StringHintParameter(text) }, null, float.MaxValue));

            }
            else
            {
                shouldUpdate = true;
                return;
            }
        }

        internal virtual string ParseElements()
        {
            if (!elements.Any())
            {
                return string.Empty;
            }

            StringBuilder sb = StringBuilderPool.Shared.Rent();
            float totalOffset = 0;

            float lastPosition = 0;
            float lastOffset = 0;

            elements.Sort();

            for (int i = 0; i < elements.Count; i++)
            {
                Element curElement = elements[i];
                if (!ShouldParse(curElement))
                {
                    continue;
                }

                ParsedData parsedData = curElement.ParsedData;
                parsedData.Offset += curElement.AdditionalLineBreaks;

                if (i != 0)
                {
                    float calcedOffset = Element.CalculateOffset(lastPosition, lastOffset, curElement.FunctionalPosition);
                    Log.Debug(calcedOffset);
                    sb.Append($"<line-height={calcedOffset}px>\n</line-height>");
                    totalOffset += calcedOffset;
                }
                else
                {
                    totalOffset += curElement.FunctionalPosition;
                }

                sb.Append(parsedData.Content);
                sb.Append(TAGCLOSER);

                totalOffset += parsedData.Offset;
                lastPosition = curElement.FunctionalPosition;
                lastOffset = parsedData.Offset;
            }

            sb.Insert(0, $"<line-height={totalOffset}px>\n");
            return StringBuilderPool.Shared.ToStringReturn(sb);
        }

        internal virtual bool ShouldParse(Element element) => element.Enabled;

        private void OnRateLimitFinished()
        {
            rateLimitActive = false;
            if (shouldUpdate)
            {
                shouldUpdate = false;
                Update();
            }

            RateLimitFinishedEventArgs args = new(this.ReferenceHub);
            Events.Events.OnRateLimitFinished(args);
        }
    }
}
