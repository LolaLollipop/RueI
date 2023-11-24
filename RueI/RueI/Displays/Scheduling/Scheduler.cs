namespace RueI.Displays.Scheduling;

using System.Diagnostics;
using eMEC;
using NorthwoodLib.Pools;
using RueI.Displays.Scheduling.Records;
using RueI.Extensions;

/// <summary>
/// Provides a means of doing batch operations.
/// </summary>
public class Scheduler
{
    public class RateLimiter
    {
        private Stopwatch consumer = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimiter"/> class.
        /// </summary>
        /// <param name="tokenLimit">The maximum number of tokens and the default number of tokens.</param>
        /// <param name="regenRate">How quickly tokens are regenerated after they have been consumed.</param>
        public RateLimiter(int tokenLimit, TimeSpan regenRate)
        {
            Tokens = tokenLimit;
            RegenRate = regenRate;
        }

        public TimeSpan RegenRate { get; set; }

        public int Tokens { get; private set; }

        public bool HasTokens => Tokens > 0;

        public void Consume()
        {
            if (Tokens > 0)
            {
                Tokens--;
                consumer.Stop();
            }
        }

        public void CalculateNewTokens()
        {
            Tokens = (int)((consumer.ElapsedMilliseconds / RegenRate.Ticks) - 0.5);
            consumer.Stop();
        }
    }

    private static readonly TimeSpan MinimumBatch = TimeSpan.FromMilliseconds(0.625);

    private readonly Cooldown rateLimiter = new();
    private readonly List<ScheduledJob> jobs = new();

    private readonly UpdateTask performTask = new();

    private readonly List<BatchJob> currentBatches = new(10);
    private readonly DisplayCore coordinator;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scheduler"/> class.
    /// </summary>
    /// <param name="coordinator">The <see cref="DisplayCore"/> to use.</param>
    public Scheduler(DisplayCore coordinator)
    {
        this.coordinator = coordinator;
    }

    /// <summary>
    /// Gets a value indicating whether or not the rate limit is currently active.
    /// </summary>
    internal bool RateLimitActive => rateLimiter.Active;

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

    /// <summary>
    /// Schedules a job.
    /// </summary>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="priority">The priority of the job, giving it additional weight when calculating.</param>
    public void Schedule(TimeSpan time, Action action, int priority)
    {
        Schedule(new ScheduledJob(DateTimeOffset.UtcNow + time, action, priority));
    }

    /// <summary>
    /// Schedules a job.
    /// </summary>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="priority">The priority of the job, giving it additional weight when calculating.</param>
    public void Schedule(Action action, TimeSpan time, int priority)
    {
        Schedule(new ScheduledJob(DateTimeOffset.UtcNow + time, action, priority));
    }

    /// <summary>
    /// Schedules a job with a priority of 1.
    /// </summary>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    public void Schedule(TimeSpan time, Action action)
    {
        Schedule(time, action, 1);
    }

    /// <summary>
    /// Delays any updates from occuring for a certain period of time.
    /// </summary>
    /// <param name="time">The amount of time to delay for.</param>
    internal void Delay(TimeSpan time)
    {
        rateLimiter.Start(time.Max(MinimumBatch));
    }

    private void UpdateBatches()
    {
        jobs.Sort();
        currentBatches.Clear();

        List<ScheduledJob> currentBatch = ListPool<ScheduledJob>.Shared.Rent(10);
        DateTimeOffset currentBatchTime = DateTimeOffset.UtcNow + MinimumBatch;

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
                currentBatch = ListPool<ScheduledJob>.Shared.Rent(10);
            }
        }

        ListPool<ScheduledJob>.Shared.Return(currentBatch);

        TimeSpan performAt = (currentBatches.First().PerformAt - DateTimeOffset.UtcNow).MaxIf(rateLimiter.Active, rateLimiter.TimeLeft);
        performTask.Start(performAt, PerformFirstBatch);
    }

    /// <summary>
    /// Immediately performs the first batch job.
    /// </summary>
    private void PerformFirstBatch()
    {
        BatchJob batchJob = currentBatches.First();

        coordinator.IgnoreUpdate = true;
        foreach (ScheduledJob job in batchJob.Jobs)
        {
            job.Action();
        }

        coordinator.IgnoreUpdate = false;
        ListPool<ScheduledJob>.Shared.Return(batchJob.Jobs);

        currentBatches.RemoveAt(0);
        rateLimiter.Start(Constants.HintRateLimit);

        coordinator.InternalUpdate();
    }
}
