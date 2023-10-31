namespace RueI
{
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
    }
}