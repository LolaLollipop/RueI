namespace RueI.Enums
{
    /// <summary>
    /// Represents the parameters that a tag needs.
    /// </summary>
    public enum TagStyle
    {
        /// <summary>
        /// Indicates that a tag does not take parameters.
        /// </summary>
        NoParams,

        /// <summary>
        /// Indicates that a tag takes a measurement as a parameter.
        /// </summary>
        Measurement,

        /// <summary>
        /// Indicates that a tag takes a color as a parameter.
        /// </summary>
        Color,
    }
}
