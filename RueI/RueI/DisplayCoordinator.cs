namespace RueI
{
    using System.Text;
    using Hints;
    using NorthwoodLib.Pools;
    using RueI.Extensions;
    using RueI.Records;

    /// <summary>
    /// Coordinates multiple PlayerDisplays.
    /// </summary>
    public class DisplayCoordinator
    {
        private List<DisplayBase> displays = new();

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
        public static DisplayCoordinator Get(ReferenceHub hub)
        {
            if (DisplayCoordinators.TryGetValue(hub, out DisplayCoordinator value))
            {
                return value;
            }
            else
            {
                return new DisplayCoordinator(hub);
            }
        }

        /// <summary>
        /// Updates this display.
        /// </summary>
        public void Update()
        {
            string text = ParseElements();
            ServerConsole.AddLog(text);
            Hub.hints.Show(new TextHint(text, new HintParameter[] { new StringHintParameter(text) }, null, 99999));
        }

        /// <summary>
        /// Adds a display to this <see cref="DisplayCoordinator"/>.
        /// </summary>
        /// <param name="display">The display to add.</param>
        internal void AddDisplay(DisplayBase display) => displays.Add(display);

        private IEnumerable<IElement> GetAllElements()
        {
            return displays.SelectMany(x => x.GetAllElements());
        }

        private string ParseElements()
        {
            List<IElement> elements = GetAllElements().ToList();

            PluginAPI.Core.Log.Info("Trying");
            if (!elements.Any())
            {
                return string.Empty;
            }

            PluginAPI.Core.Log.Info(elements.Count.ToString());
            StringBuilder sb = StringBuilderPool.Shared.Rent();
            float totalOffset = 0;

            float lastPosition = 0;
            float lastOffset = 0;

            elements.Sort();

            for (int i = 0; i < elements.Count; i++)
            {
                IElement curElement = elements[i];

                ParsedData parsedData = curElement.ParsedData;

                float funcPos = curElement.GetFunctionalPosition();

                if (i != 0)
                {
                    float calcedOffset = ElementHelpers.CalculateOffset(lastPosition, lastOffset, funcPos);
                    sb.Append($"<line-height={calcedOffset}px>\n");
                    totalOffset += calcedOffset;
                }
                else
                {
                    totalOffset += funcPos;
                }

                sb.Append(parsedData.Content);
                sb.Append(Constants.TAGCLOSER);

                totalOffset += parsedData.Offset;
                lastPosition = funcPos;
                lastOffset = parsedData.Offset;
            }

            return $"<line-height={totalOffset}px>\n" + StringBuilderPool.Shared.ToStringReturn(sb);
        }
    }
}