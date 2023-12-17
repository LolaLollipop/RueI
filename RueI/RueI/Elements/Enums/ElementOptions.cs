namespace RueI.Elements.Enums;

/// <summary>
/// Provides options for elements.
/// </summary>
[Flags]
public enum ElementOptions
{
    /// <summary>
    /// Indicates whether or not noparse ignores escape sequences like \r, \u, and \n.
    /// </summary>
    NoparseIgnoresEscape = 1 << 0,

    /// <summary>
    /// Indicates whether or not the vertical spacing of an element affects the baseline.
    /// </summary>
    PreserveSpacing = 1 << 1,

    /// <summary>
    /// Gets the default element settings.
    /// </summary>
    Default = NoparseIgnoresEscape,

    /// <summary>
    /// Gets the vanilla options for hints.
    /// </summary>
    Vanilla = PreserveSpacing,
}