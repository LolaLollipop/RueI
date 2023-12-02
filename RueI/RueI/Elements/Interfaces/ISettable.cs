namespace RueI.Elements.Interfaces;

using RueI.Elements;

/// <summary>
/// Defines an element that can be set.
/// </summary>
public interface ISettable
{
    /// <summary>
    /// Sets the content of this element.
    /// </summary>
    /// <param name="text">The new element.</param>
    public void Set(string text);
}
