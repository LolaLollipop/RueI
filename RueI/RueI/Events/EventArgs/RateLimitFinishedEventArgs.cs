namespace RueI.Events
{
    /// <summary>
    /// Contains all information after a player's <see cref="DisplayCoordinator"/> is updated.
    /// </summary>
    public class DisplayUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="referenceHub">The hub of the player.</param>
        /// <param name="displayCord">The DisplayCoordinator .</param>
        public DisplayUpdatedEventArgs(ReferenceHub referenceHub, DisplayCoordinator displayCord)
        {
            ReferenceHub = referenceHub;
            DisplayCoordinator = displayCord;
        }

        /// <summary>
        /// Gets the ReferenceHub of the player.
        /// </summary>
        public ReferenceHub ReferenceHub { get; }

        /// <summary>
        /// Gets the DisplayCoordinator of the player.
        /// </summary>
        public DisplayCoordinator DisplayCoordinator { get; }
    }
}
