namespace RueI
{
    using System.Text;
    using System.Xml.Linq;
    using Hints;
    using NorthwoodLib.Pools;
    using RueI.Records;

    /// <summary>
    /// Coordinates multiple PlayerDisplays.
    /// </summary>
    public class DisplayCoordinator
    {
        private List<Display> displays = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayCoordinator"/> class.
        /// <param name="hub">The hub to create the display for.</param>
        private DisplayCoordinator(ReferenceHub hub)
        {
            this.Hub = hub;

            DisplayCoordinators.Add(hub, this);
        }

        /// <summary>
        /// Gets a dictionary containing the DisplayCoordinators for each ReferenceHub.
        /// </summary>
        internal static Dictionary<ReferenceHub, DisplayCoordinator> DisplayCoordinators { get; } = new();

        /// <summary>
        /// Gets the <see cref="ReferenceHub"/> that this display is for.
        /// </summary>
        internal ReferenceHub Hub { get; }

        /// <summary>
        /// Gets a DisplayCoordinator from a <see cref="ReferenceHub"/>, or creates it if it doesn't exist.
        /// </summary>
        /// <param name="hub">The hub to get the display for.</param>
        /// <returns>The DisplayCoordinator.</returns>
        public static DisplayCoordinator Get(ReferenceHub hub) {
            if (DisplayCoordinators.TryGetValue(hub, out DisplayCoordinator value))
            {
                return value;
            }
            else
            {
                return new DisplayCoordinator(hub);
            }
        }

        public void Update()
        {
            string text = ParseElements();
            Hub.hints.Show(new TextHint(text, new HintParameter[] { new StringHintParameter(text) }, null, float.MaxValue));
        }

        internal void AddDisplay(Display display) => displays.Add(display);

        private IEnumerable<Element> GetAllElements()
        {
            foreach (Display display in displays)
            {
                foreach (Element element in display.Elements)
                {
                    if (display.ShouldParse(element))
                    {
                        yield return element;
                    }
                }
            }
        }

        private string ParseElements()
        {
            List<Element> elements = displays.SelectMany((display) =>
            {
                return display.Elements;
            }).ToList();

            ServerConsole.AddLog("Running RueI :)");
            if (!elements.Any())
            {
                return string.Empty;
            }

            ServerConsole.AddLog(elements.Count.ToString());
            StringBuilder sb = StringBuilderPool.Shared.Rent();
            float totalOffset = 0;

            float lastPosition = 0;
            float lastOffset = 0;

            elements.Sort();

            for (int i = 0; i < elements.Count; i++)
            {
                Element curElement = elements[i];

                ParsedData parsedData = curElement.ParsedData;
                parsedData.Offset += curElement.AdditionalLineBreaks;

                if (i != 0)
                {
                    float calcedOffset = Element.CalculateOffset(lastPosition, lastOffset, curElement.FunctionalPosition);
                    sb.Append($"<line-height={calcedOffset}px>\n</line-height>");
                    totalOffset += calcedOffset;
                }
                else
                {
                    totalOffset += curElement.FunctionalPosition;
                }

                sb.Append(parsedData.Content);
                sb.Append(Constants.TAGCLOSER);

                totalOffset += parsedData.Offset;
                lastPosition = curElement.FunctionalPosition;
                lastOffset = parsedData.Offset;
            }

            return $"<line-height={totalOffset}px>\n" + StringBuilderPool.Shared.ToStringReturn(sb);
        }
    }
}