using System.Diagnostics;

namespace RueI.Displays.Scheduling;

/// <summary>
/// Provides a way to ratelimit actions or detect ratelimits.
/// </summary>
public class RateLimiter
{
    private DateTimeOffset lastConsumed = DateTime.UtcNow;
    private double progress;

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

    /// <summary>
    /// Gets or sets the regeneration rate for this ratelimiter.
    /// </summary>
    public TimeSpan RegenRate { get; set; }

    /// <summary>
    /// Gets the limit on tokens in this ratelimiter.
    /// </summary>
    public int TokenLimit { get; private set; }

    /// <summary>
    /// Gets the number of tokens available in this ratelimiter.
    /// </summary>
    public int Tokens { get; private set; }

    /// <summary>
    /// Gets a value indicating whether or not this ratelimiter has a token available.
    /// </summary>
    public bool HasTokens => Tokens > 0;

    /// <summary>
    /// Consumes a token from this ratelimiter.
    /// </summary>
    public void Consume()
    {
        CalculateNewTokens();
        if (Tokens > 0)
        {
            Tokens--;
        }
    }

    /// <summary>
    /// Calculates the number of new tokens for this ratelimiter.
    /// </summary>
    public void CalculateNewTokens()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        TimeSpan difference = now - lastConsumed;
        double diff = (difference.TotalMilliseconds / RegenRate.TotalMilliseconds) + progress;

        int newTokens = Math.Min((int)Math.Floor(diff), TokenLimit - Tokens);
        progress = diff - newTokens;
        Tokens += newTokens;
    }
}