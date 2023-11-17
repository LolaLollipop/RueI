namespace RueI.Records;

/// <summary>
/// Defines a batch job.
/// </summary>
internal class BatchJob
{
    // TODO: make this not suck
    internal List<ScheduledJob> jobs;

    internal DateTimeOffset PerformAt { get; set; }

    public BatchJob(List<ScheduledJob> jobs, DateTimeOffset performAt)
    {
        this.jobs = jobs;
        PerformAt = performAt;
    }
}
