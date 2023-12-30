namespace RueI.Displays.Scheduling;

/// <summary>
/// Represents a reference to any number of <see cref="Records.ScheduledJob"/>.
/// </summary>
/// <remarks>
/// A <see cref="JobToken"/> provides a unique identifier for a <see cref="Records.ScheduledJob"/> within any number of <see cref="Scheduler"/>s. In other words, a <see cref="JobToken"/> can reference multiple (or no) <see cref="Records.ScheduledJob"/>, but only a single <see cref="Records.ScheduledJob"/> with the given <see cref="JobToken"/> can exist in a <see cref="Scheduler"/>.
/// </remarks>
/// <seealso cref="Records.ScheduledJob"/>
public class JobToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JobToken"/> class.
    /// </summary>
    public JobToken()
    {
    }
}
