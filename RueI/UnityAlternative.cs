namespace RueI;

using eMEC;
using MEC;
using RueI.Extensions;
using UnityEngine;
using static RueI.NonUnityProvider;

/// <summary>
/// Defines the base class for a provider of methods that may or may not use Unity.
/// </summary>
public abstract class UnityAlternative
{
    /// <summary>
    /// Represents a generalized handler for an async operation.
    /// </summary>
    public interface IAsyncOperation : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether or not this operation is handling.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// Cancels this operation.
        /// </summary>
        public void Cancel();
    }

    /// <summary>
    /// Gets the current <see cref="UnityAlternative"/> of the application.
    /// </summary>
    public static UnityAlternative Provider { get; } = GetProvider();

    /// <summary>
    /// Logs a message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public abstract void Log(string message);

    /// <summary>
    /// Performs an async operation.
    /// </summary>
    /// <param name="span">How long until the action should be ran.</param>
    /// <param name="action">The action to run when finished.</param>
    public abstract IAsyncOperation PerformAsync(TimeSpan span, Action action);

    private static UnityAlternative GetProvider()
    {
        try
        {
            _ = Object.FindObjectOfType<ReferenceHub>();
            return new UnityProvider();
        }
        catch(Exception)
        {
            return new NonUnityProvider();
        }
    }
}

/// <summary>
/// Provides non-Unity alternatives for the <see cref="UnityProvider"/> of the application.
/// </summary>
public class NonUnityProvider : UnityAlternative
{
    /// <inheritdoc/>
    public override void Log(string message) => Console.WriteLine(message);

    /// <inheritdoc/>
    public override IAsyncOperation PerformAsync(TimeSpan span, Action action) => new TaskAsyncOperation(span, action);

    /// <summary>
    /// Represents an async operation using a <see cref="Task"/>.
    /// </summary>
    public class TaskAsyncOperation : IAsyncOperation
    {
        private readonly Task task;
        private readonly CancellationTokenSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAsyncOperation"/> class, and then immediately runs.
        /// </summary>
        /// <param name="span">How long until the action should be ran.</param>
        /// <param name="action">The action to run when finished.</param>
        public TaskAsyncOperation(TimeSpan span, Action action)
        {
            source = new();
            task = Task.Run(async () =>
            {
                await Task.Delay(span);
                action();
            });
        }

        /// <inheritdoc/>
        public bool IsRunning => !task.IsCompleted;

        /// <inheritdoc/>
        public void Cancel() => source.Cancel();

        /// <summary>
        /// Disposes this async operation.
        /// </summary>
        public void Dispose() => source.Dispose();
    }
}

/// <summary>
/// Provides Unity methods for the application.
/// </summary>
public class UnityProvider : UnityAlternative
{
    /// <inheritdoc/>
    public override void Log(string message) => ServerConsole.AddLog(message, ConsoleColor.Yellow);

    /// <inheritdoc/>
    public override IAsyncOperation PerformAsync(TimeSpan span, Action action) => new MECAsyncOperation(span, action);

    /// <summary>
    /// Represents an async operation using a <see cref="Task"/>.
    /// </summary>
    public class MECAsyncOperation : IAsyncOperation
    {
        private readonly CoroutineHandle handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MECAsyncOperation"/> class, and then immediately runs.
        /// </summary>
        /// <param name="span">How long until the action should be ran.</param>
        /// <param name="action">The action to run when finished.</param>
        public MECAsyncOperation(TimeSpan span, Action action)
        {
            handle = Timing.CallDelayed(((float)span.TotalSeconds).Max(0), action);
        }

        /// <inheritdoc/>
        public bool IsRunning => handle.IsRunning;

        /// <inheritdoc/>
        public void Cancel() => Timing.KillCoroutines(handle);

        /// <summary>
        /// Disposes this MEC operation.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
