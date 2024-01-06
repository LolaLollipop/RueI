namespace RueI.Extensions;

using RueI.Elements;
using RueI.Displays.Interfaces;

/// <summary>
/// Provides extensions and helpers for working with elements.
/// </summary>
public static class ElementHelpers
{
    /// <summary>
    /// Filters out all of the disabled <see cref="Element"/>s in an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="elements">The elements to filter.</param>
    /// <returns>The filtered <see cref="IEnumerable{T}"/>.</returns>
    public static IEnumerable<Element> FilterDisabled(this IEnumerable<Element> elements) => elements.Where(x => x.Enabled);

    /// <summary>
    /// Gets the functional (un-scaled) position of an element.
    /// </summary>
    /// <param name="element">The element to get the position for.</param>
    /// <returns>The un-scaled position.</returns>
    public static float GetFunctionalPosition(this Element element)
    {
        if (element.Options.HasFlagFast(Elements.Enums.ElementOptions.UseFunctionalPosition))
        {
            return element.Position;
        }
        else
        {
            return Ruetility.ScaledPositionToFunctional(element.Position);
        }
    }
}