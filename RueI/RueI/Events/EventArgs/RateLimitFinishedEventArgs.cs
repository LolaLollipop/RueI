namespace RueI.Events;

/// <summary>
/// Contains all information after a player's <see cref="DisplayCoordinator"/> is updated.
/// </summary>
public class DisplayUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayUpdatedEventArgs"/> class.
    /// </summary>
    /// <param name="referenceHub">The hub of the player.</param>
    /// <param name="displayCord">The DisplayCore .</param>
    public DisplayUpdatedEventArgs(ReferenceHub referenceHub, DisplayCore displayCord)
    {
        ReferenceHub = referenceHub;
        DisplayCoordinator = displayCord;
    }

    /// <summary>
    /// Gets the ReferenceHub of the player.
    /// </summary>
    public ReferenceHub ReferenceHub { get; }

    /// <summary>
    /// Gets the DisplayCore of the player.
    /// </summary>
    public DisplayCore DisplayCoordinator { get; }
}
