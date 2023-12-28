namespace RueI.Displays.Scheduling;

using RueI.Elements;

/// <summary>
/// Represents a <see cref="IElemReference{T}"/> with an associated <see cref="Scheduling.JobToken"/>.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
public class TimedElemRef<T> : IElemReference<T>
    where T : Element
{
    /// <summary>
    /// Gets the <see cref="Scheduling.JobToken"/> for this element reference.
    /// </summary>
    public JobToken JobToken { get; } = new();
}
