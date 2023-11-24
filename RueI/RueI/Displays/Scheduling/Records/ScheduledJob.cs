namespace RueI.Displays.Scheduling.Records;

/// <summary>
/// Defines a scheduled job for a <see cref="Scheduler"/>.
/// </summary>
public class ScheduledJob : IComparable<ScheduledJob>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledJob"/> class.
    /// </summary>
    /// <param name="finishAt">When the job should be performed.</param>
    /// <param name="action">The action to perform when done.</param>
    /// <param name="priority">The priority of the element.</param>
    internal ScheduledJob(DateTimeOffset finishAt, Action action, int priority)
    {
        FinishAt = finishAt;
        Action = action;
        Priority = priority;
    }

    /// <summary>
    /// Gets the action that will be performed when this job is done.
    /// </summary>
    internal Action Action { get; private set; }

    /// <summary>
    /// Gets when the job would optimally be performed at.
    /// </summary>
    internal DateTimeOffset FinishAt { get; private set; }

    /// <summary>
    /// Gets the priority of the element.
    /// </summary>
    internal int Priority { get; private set; } = 1;

    /// <summary>
    /// Compares this <see cref="ScheduledJob"/> to another job.
    /// </summary>
    /// <param name="other">The other <see cref="ScheduledJob"/>.</param>
    /// <returns>An int indicating whether or not the <see cref="DateTimeOffset"/> of this job comes before or after the other.</returns>
    public int CompareTo(ScheduledJob other) => this.FinishAt.CompareTo(other.FinishAt);
}
