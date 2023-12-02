namespace RueI.Displays.IDGenerator;

/// <summary>
/// Generates new, unique IDs.
/// </summary>
internal static class IDGenerator
{
    private static int nextID = 0;

    /// <summary>
    /// Gets a new ID.
    /// </summary>
    /// <returns>A new ID.</returns>
    internal static int GetID() => nextID++;
}