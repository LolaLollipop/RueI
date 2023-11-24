namespace RueI.Elements;

using RueI.Parsing.Records;

/// <summary>
/// Represents the base interface for all elements.
/// </summary>
public interface IElement
{
    /// <summary>
    /// Gets or sets a value indicating whether or not this element is enabled and will show.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets the data used for parsing.
    /// </summary>
    /// <remarks>This contains information used to ensure that multiple elements can be displayed at once. To obtain this, you should almost always use <see cref="Parser.Parse"/>.</remarks>
    public ParsedData ParsedData { get; }

    /// <summary>
    /// Gets or sets the position of the element on a scale from 0-1000, where 0 represents the bottom of the screen and 1000 represents the top.
    /// </summary>
    public float Position { get; set; }

    /// <summary>
    /// Gets or sets the priority of the hint (determining if it shows above another hint).
    /// </summary>
    public int ZIndex { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Parser"/> currently in use by this <see cref="IElement"/>.
    /// </summary>
    /// <remarks>Implementations should default this to <see cref="Parser.DefaultParser"/>.</remarks>
    public Parser Parser { get; set; }
}