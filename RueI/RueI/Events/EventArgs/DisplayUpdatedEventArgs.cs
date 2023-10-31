namespace RueI.Events
{
    /// <summary>
    /// Contains all information after a player's hint rate limit is finished.
    /// </summary>
    public class RateLimitFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitFinishedEventArgs"/> class.
        /// </summary>
        /// <param name="referenceHub">The hub that the rate limit is finished for.</param>
        public RateLimitFinishedEventArgs(ReferenceHub referenceHub)
        {
            ReferenceHub = referenceHub;
        }

        /// <summary>
        /// Gets the ReferenceHub that the hint rate limit is finished for.
        /// </summary>
        public ReferenceHub ReferenceHub { get; }
    }
}
