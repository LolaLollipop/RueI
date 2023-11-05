namespace RueI.Records
{
    using NorthwoodLib.Pools;

    /// <summary>
    /// Defines a batch job.
    /// </summary>
    internal class BatchJob
    {
        internal List<ScheduledJob> jobs;

        internal DateTimeOffset PerformAt { get; set; }

        public BatchJob(List<ScheduledJob> jobs, DateTimeOffset performAt)
        {
            this.jobs = jobs;
            PerformAt = performAt;
        }
    }
}
