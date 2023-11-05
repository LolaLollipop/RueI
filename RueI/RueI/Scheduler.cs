namespace RueI.Displays
{
    using eMEC;
    using NorthwoodLib.Pools;
    using RueI.Extensions;
    using RueI.Records;

    /// <summary>
    /// Provides a means of doing batch operations.
    /// </summary>
    public class Scheduler
    {
        private static TimeSpan minimumBatch = TimeSpan.FromMilliseconds(0.625);

        private readonly Cooldown rateLimiter = new();
        private readonly List<ScheduledJob> jobs = new();

        private readonly UpdateTask performTask = new();

        private List<BatchJob> currentBatches = new(10);
        private DisplayCoordinator coordinator;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="coordinator">The <see cref="DisplayCoordinator"/> to use.</param>
        public Scheduler(DisplayCoordinator coordinator)
        {
            this.coordinator = coordinator;
        }

        /// <summary>
        /// Calculates the weighted time for a list of jobs to be performed.
        /// </summary>
        /// <param name="jobs">The jobs.</param>
        /// <returns>The weighted <see cref="DateTimeOffset"/> of all of the jobs.</returns>
        public static DateTimeOffset CalculateWeighted(IEnumerable<ScheduledJob> jobs)
        {
            long currentSum = 0;
            int prioritySum = 0;

            foreach (ScheduledJob job in jobs)
            {
                currentSum += job.FinishAt.ToUnixTimeMilliseconds();
                prioritySum += job.Priority;
            }

            return DateTimeOffset.FromUnixTimeMilliseconds(currentSum / prioritySum);
        }

        /// <summary>
        /// Schedules a job.
        /// </summary>
        /// <param name="job">The job to schedule.</param>
        public void Schedule(ScheduledJob job)
        {
            jobs.Add(job);
            UpdateBatches();
        }

        private void UpdateBatches()
        {
            jobs.Sort();
            currentBatches.Clear();

            List<ScheduledJob> currentBatch = ListPool<ScheduledJob>.Shared.Rent(10);
            DateTimeOffset currentBatchTime = DateTimeOffset.UtcNow + minimumBatch;

            foreach (ScheduledJob job in jobs)
            {
                if (job.FinishAt < currentBatchTime)
                {
                    currentBatch.Add(job);
                }
                else
                {
                    BatchJob finishedBatch = new(currentBatch, CalculateWeighted(currentBatch));
                    currentBatches.Add(finishedBatch);
                }
            }

            ListPool<ScheduledJob>.Shared.Return(currentBatch);

            TimeSpan performAt = (currentBatches.First().PerformAt - DateTimeOffset.UtcNow).MaxIf(rateLimiter.Active, rateLimiter.TimeLeft);
            performTask.Start(performAt, PerformFirstBatch);
        }

        /// <summary>
        /// Immediately performs the first batch job.
        /// </summary>
        /// <param name="batchJob">The job to perform.</param>
        private void PerformFirstBatch()
        {
            BatchJob batchJob = currentBatches.First();

            coordinator.IgnoreUpdate = true;
            foreach (ScheduledJob job in batchJob.jobs)
            {
                job.Action();
            }

            coordinator.IgnoreUpdate = false;

            currentBatches.RemoveAt(0);
            rateLimiter.Start(Constants.HintRateLimit);
        }
    }
}
