namespace RueI.Elements.Enums;

/// <summary>
/// Provides options for elements.
/// </summary>
[Flags]
public enum ElementOptions
{
    /// <summary>
    /// Indicates whether or not noparse parses escape sequences like \r, \u, and \n.
    /// </summary>
    NoparseParsesEscape = 1 << 0,

    /// <summary>
    /// Indicates whether or not the vertical spacing of an element affects the baseline.
    /// </summary>
    PreserveSpacing = 1 << 1,

    /// <summary>
    /// Indicates whether or not to automatically use functional positioning for the element.
    /// </summary>
    UseFunctionalPosition = 1 << 2,

    /// <summary>
    /// Gets the default element settings.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Gets the vanilla options for hints.
    /// </summary>
    Vanilla = PreserveSpacing | UseFunctionalPosition | NoparseParsesEscape,
}