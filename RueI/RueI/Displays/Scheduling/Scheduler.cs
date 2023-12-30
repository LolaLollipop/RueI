namespace RueI.Displays.Scheduling;

using Utils.NonAllocLINQ;

using eMEC;
using RueI.Displays.Scheduling.Records;
using RueI.Extensions;

/// <summary>
/// Provides a means of doing batch operations.
/// </summary>
/// <remarks>
/// The <see cref="Scheduler"/> is a powerful class that enables "batch operations". This means that multiple updates to a display can happen at once, helping to avoid the hint ratelimit.
/// </remarks>
public class Scheduler
{
    private static readonly TimeSpan MinimumBatch = TimeSpan.FromMilliseconds(625);

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

    private static DateTimeOffset Now => DateTimeOffset.UtcNow;

    /// <summary>
    /// Calculates the weighted time for a list of jobs to be performed.
    /// </summary>
    /// <param name="jobs">The jobs.</param>
    /// <returns>The weighted <see cref="DateTimeOffset"/> of all of the jobs.</returns>
    public static DateTimeOffset CalculateWeighted(IEnumerable<ScheduledJob> jobs)
    {
        if (!jobs.Any())
        {
            return default;
        }

        long currentSum = 0;
        int prioritySum = 0;

        foreach (ScheduledJob job in jobs)
        {
            currentSum += job.FinishAt.ToUnixTimeMilliseconds() * job.Priority;
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
        if (job.Token == null || !jobs.Any(x => x.Token == job.Token))
        {
            jobs.Add(job);
            UpdateBatches();
        }
    }

    /// <summary>
    /// Schedules an uncancellable update job.
    /// </summary>
    /// <param name="time">How long into the future to update at.</param>
    /// <param name="priority">The priority of the job, giving it additional weight when calculating.</param>
    public void ScheduleUpdate(TimeSpan time, int priority)
    {
        jobs.Add(new(Now + time, () => { }, priority));
        UpdateBatches();
    }

    /// <summary>
    /// Schedules a job.
    /// </summary>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="priority">The priority of the job, giving it additional weight when calculating.</param>
    /// <param name="token">An optional token to assign to the <see cref="ScheduledJob"/>.</param>
    public void Schedule(TimeSpan time, Action action, int priority, JobToken? token = null)
    {
        Schedule(new ScheduledJob(Now + time, action, priority, token));
    }

    /// <summary>
    /// Schedules a job.
    /// </summary>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="priority">The priority of the job, giving it additional weight when calculating.</param>
    /// <param name="token">An optional token to assign to the <see cref="ScheduledJob"/>.</param>
    public void Schedule(Action action, TimeSpan time, int priority, JobToken? token = null)
    {
        Schedule(new ScheduledJob(Now + time, action, priority, token));
    }

    /// <summary>
    /// Schedules a job with a priority of 1.
    /// </summary>
    /// <param name="time">How long into the future to run the action at.</param>
    /// <param name="action">The <see cref="Action"/> to run.</param>
    /// <param name="token">An optional token to assign to the <see cref="ScheduledJob"/>.</param>
    public void Schedule(TimeSpan time, Action action, JobToken? token = null)
    {
        Schedule(time, action, 1, token);
    }

    /// <summary>
    /// Attempts to kill a single job using the <see cref="JobToken"/>.
    /// </summary>
    /// <param name="token">The <see cref="JobToken"/> to use as a reference.</param>
    public void KillJob(JobToken token)
    {
        int index = jobs.FindIndex(x => x.Token == token);
        if (index != -1)
        {
            jobs.RemoveAt(index);
        }
    }

    /// <summary>
    /// Attempts to kill all jobs that have the <see cref="JobToken"/>.
    /// </summary>
    /// <param name="token">The <see cref="JobToken"/> to use as a reference.</param>
    public void KillMultiple(JobToken token) => jobs.RemoveAll(x => x.Token == token);

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
        if (!jobs.Any())
        {
            return;
        }

        jobs.Sort();
        currentBatches.Clear();

        List<ScheduledJob> currentBatch = new();
        DateTimeOffset currentBatchTime = jobs.First().FinishAt + MinimumBatch;

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
                currentBatch = new()
                {
                    job,
                };
            }
        }

        if (currentBatch.Count != 0)
        {
            BatchJob finishedBatch = new(currentBatch, CalculateWeighted(currentBatch));

            currentBatches.Add(finishedBatch);
        }

        TimeSpan performAt = (currentBatches.First().PerformAt - Now).MaxIf(rateLimiter.Active, rateLimiter.TimeLeft);
        ServerConsole.AddLog("Starting");
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

        currentBatches.RemoveAt(0);
        rateLimiter.Start(Constants.HintRateLimit);

        coordinator.InternalUpdate();
    }
}
