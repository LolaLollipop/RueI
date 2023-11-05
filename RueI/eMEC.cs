#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace eMEC
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using MEC;
    using RueI.Extensions;
    using UnityEngine;

    /// <summary>
    /// Provides extensions for working with MEC. The primary purpose is to provide better nullable functionality.
    /// </summary>
    public static class MECExtensions
    {
        /// <summary>
        /// Kills a coroutine.
        /// </summary>
        /// <param name="handle">The handle to kill.</param>
        public static void Kill(this CoroutineHandle handle) => Timing.KillCoroutines(handle);

        /// <summary>
        /// Gets whether or not a coroutine is running.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>A bool indicating whether or not the coroutine is running.</returns>
        public static bool IsRunning(this CoroutineHandle handle) => Timing.IsRunning(handle);

        /// <summary>
        /// Gets whether or not a coroutine is running or paused.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>A bool indicating whether or not the coroutine is running or paused.</returns>
        public static bool IsRunningOrPaused(this CoroutineHandle handle) => Timing.IsRunning(handle) || Timing.IsAliveAndPaused(handle);
    }

    /// <summary>
    /// Represents anything that deals with Tasks.
    /// </summary>
    public interface ITaskable
    {
        /// <summary>
        /// Recursvely cleans up this <see cref="ITaskable"/>.
        /// </summary>
        public void CleanUp();

        /// <summary>
        /// Recursively loops through the pool, or performs the action if this is a Task.
        /// </summary>
        /// <param name="action">The <see cref="Action{TaskBase}"/> to perform.</param>
        public void DescendOrPerform(Action<TaskBase> action);
    }

    /// <summary>
    /// Represents a task that runs a method when finished.
    /// </summary>
    public class UpdateTask : TaskBase
    {
        private readonly Stopwatch stopwatch = new();

        /// <summary>
        /// Gets or sets a method that will be run when the task is finished.
        /// </summary>
        public Action? Action { get; set; }

        /// <summary>
        /// Gets the length of the task.
        /// </summary>
        public TimeSpan? Length { get; private set; }

        /// <summary>
        /// Gets the amount of time left until the task is finished.
        /// </summary>
        public TimeSpan? TimeLeft => Length - stopwatch.Elapsed;

        /// <summary>
        /// Gets how long the task has been running for.
        /// </summary>
        public TimeSpan? ElapsedTime => stopwatch.Elapsed;

        /// <summary>
        /// Gets how long the task has been running for.
        /// </summary>
        public DateTimeOffset? FinishesAt => DateTimeOffset.UtcNow + TimeLeft;

        /// <inheritdoc/>
        [MemberNotNullWhen(returnValue: true, nameof(TimeLeft))]
        [MemberNotNullWhen(returnValue: true, nameof(ElapsedTime))]
        [MemberNotNullWhen(returnValue: true, nameof(Action))]
        [MemberNotNullWhen(returnValue: true, nameof(Length))]
        public override bool IsRunning => ch?.IsRunningOrPaused() ?? false;

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <param name="length">How long to run the task for.</param>
        /// <param name="action">The action to run when finished.</param>
        public void Start(TimeSpan length, Action action)
        {
            End();
            Action = action;
            Length = length;
            stopwatch.Start();
            ch = Timing.CallDelayed(((float)length.TotalSeconds).Max(0), () =>
            {
                Action();
                ResetState();
            });
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <param name="length">In seconds, how long to run the action for. </param>
        /// <param name="action">The action to run when finished.</param>
        public void Start(float length, Action action) => Start(TimeSpan.FromSeconds(length), action);

        /// <summary>
        /// Adds a certain amount of time to the length of the task, if it is running.
        /// </summary>
        /// <param name="toAdd">An amount of time to add.</param>
        /// <remarks>The timespan can be negative. If the new length is less than the elapsed time, the task will immediately finish.</remarks>
        public void AddLength(TimeSpan toAdd)
        {
            if (IsRunning)
            {
                ChangeLength(Length.Value + toAdd);
            }
        }

        /// <summary>
        /// Subtracts a certain amount of time from the length of the task, if it is running.
        /// </summary>
        /// <param name="toSubtract">An amount of time to subtract.</param>
        /// <remarks>If the new length is less than the elapsed time, the task will immediately finish.</remarks>
        public void SubtractLength(TimeSpan toSubtract)
        {
            if (IsRunning)
            {
                ChangeLength(Length.Value - toSubtract);
            }
        }

        /// <summary>
        /// Sets the length of the task, if it is running.
        /// </summary>
        /// <param name="newLength">The new length.</param>
        /// <remarks>If the new length is less than the elapsed time, the task will immediately finish.</remarks>
        public void ChangeLength(TimeSpan newLength)
        {
            if (IsRunning)
            {
                TimeSpan newTime = Length.Value - newLength;

                if (newTime > TimeSpan.Zero)
                {
                    ch?.Kill();
                    Start(newTime, Action);
                }
                else
                {
                    Action();
                    ResetState();
                }
            }
        }

        /// <summary>
        /// Pauses the task.
        /// </summary>
        public void Pause()
        {
            if (IsRunning)
            {
                Timing.PauseCoroutines(ch.Value);
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// Resumes the task if it is paused.
        /// </summary>
        public void Resume()
        {
            if (IsRunning)
            {
                Timing.ResumeCoroutines(ch.Value);
                stopwatch.Start();
            }
        }

        /// <summary>
        /// Resets the state of the task to before it was started.
        /// </summary>
        protected override void ResetState()
        {
            stopwatch.Reset();
            base.ResetState();
        }
    }

    /// <summary>
    /// Provides the base class for all tasks.
    /// </summary>
    public abstract class TaskBase : ITaskable
    {
        protected CoroutineHandle? ch;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBase"/> class.
        /// </summary>
        /// <param name="pool">The pool to add Task to.</param>
        public TaskBase(TaskPool pool)
        {
            pool.Add(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBase"/> class.
        /// </summary>
        public TaskBase()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TaskBase"/> class.
        /// </summary>
        ~TaskBase()
        {
            CleanUp();
        }

        /// <summary>
        /// Gets a value indicating whether or not this task is currently running.
        /// </summary>
        [MemberNotNullWhen(returnValue: true, nameof(ch))]
        public abstract bool IsRunning { get; }

        /// <inheritdoc/>
        public void CleanUp() => End();

        /// <summary>
        /// Ends the task, without calling the method.
        /// </summary>
        public virtual void End()
        {
            ch?.Kill();
            ResetState();
        }

        /// <summary>
        /// Performs an action on this task.
        /// </summary>
        /// <param name="action">The method to perform.</param>
        public void DescendOrPerform(Action<TaskBase> action) => action(this);

        protected virtual void ResetState()
        {
            ch = null;
        }
    }

    /// <summary>
    /// Manages a number of ITaskables and tasks.
    /// </summary>
    public class TaskPool : Collection<ITaskable>, ITaskable
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="TaskPool"/> class.
        /// </summary>
        ~TaskPool() => CleanUp();

        public void CleanUp()
        {
            foreach (ITaskable killable in this)
            {
                killable.CleanUp();
            }
        }

        public void DescendOrPerform(Action<TaskBase> action)
        {
            foreach (ITaskable taskable in this)
            {
                taskable.DescendOrPerform(action);
            }
        }
    }

    /// <summary>
    /// Provides a way to implement a cooldown easily.
    /// </summary>
    public class Cooldown
    {
        private Stopwatch stopwatch = new();

        /// <summary>
        /// Gets or sets the current length of the cooldown.
        /// </summary>
        public TimeSpan Length { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Gets the amount of time left for the cooldown.
        /// </summary>
        public TimeSpan TimeLeft => Length - stopwatch.Elapsed;

        /// <summary>
        /// Gets a value indicating whether or not the cooldown is active.
        /// </summary>
        public bool Active => stopwatch.Elapsed >= Length;

        /// <summary>
        /// Starts the cooldown.
        /// </summary>
        /// <param name="length">How long the cooldown should last.</param>
        public void Start(TimeSpan length)
        {
            Length = length;
            stopwatch.Start();
        }

        /// <summary>
        /// Starts the cooldown.
        /// </summary>
        /// <param name="length">In seconds, how long the cooldown should last.</param>
        public void Start(float length) => this.Start(TimeSpan.FromSeconds(length));

        /// <summary>
        /// Pauses the cooldown.
        /// </summary>
        public void Pause()
        {
            stopwatch.Stop();
        }

        /// <summary>
        /// Resume the cooldown if it is paused.
        /// </summary>
        public void Resume()
        {
            stopwatch.Start();
        }
    }
}
