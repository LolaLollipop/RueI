namespace RueI.Events;

using RueI.Displays;

/// <summary>
/// Contains all information after a player's <see cref="DisplayCore"/> is updated.
/// </summary>
public class DisplayUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayUpdatedEventArgs"/> class.
    /// </summary>
    /// <param name="referenceHub">The hub of the player.</param>
    /// <param name="displayCord">The <see cref="DisplayCore"/> to use..</param>
    public DisplayUpdatedEventArgs(ReferenceHub referenceHub, DisplayCore displayCord)
    {
        ReferenceHub = referenceHub;
        DisplayCore = displayCord;
    }

    /// <summary>
    /// Gets the ReferenceHub of the player.
    /// </summary>
    public ReferenceHub ReferenceHub { get; }

    /// <summary>
    /// Gets the DisplayCore of the player.
    /// </summary>
    public DisplayCore DisplayCore { get; }
}
