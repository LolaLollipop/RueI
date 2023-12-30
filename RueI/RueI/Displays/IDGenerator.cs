namespace RueI.Displays.IDGenerator;

/// <summary>
/// Generates new, unique IDs.
/// </summary>
/// <remarks>
/// This class is not thread safe.
/// </remarks>
internal static class IDGenerator
{
    private static int nextID = 0;

    /// <summary>
    /// Gets a new and unique ID.
    /// </summary>
    /// <returns>A new, unique ID.</returns>
    internal static int GetID() => nextID++;
}