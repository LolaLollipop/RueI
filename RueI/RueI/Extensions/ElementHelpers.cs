namespace RueI.Extensions;

using RueI.Interfaces;

/// <summary>
/// Provides extensions and helpers for working with elements.
/// </summary>
public static class ElementHelpers
{
    /// <summary>
    /// Adds an <see cref="IElement"/> to a <see cref="IElementContainer"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IElement"/>.</typeparam>
    /// <param name="element">The element to add.</param>
    /// <param name="container">The <see cref="IElementContainer"/> to add to.</param>
    /// <returns>A reference to this element.</returns>
    public static T AddTo<T>(this T element, IElementContainer container)
        where T : IElement
    {
        element.AddTo(container.Elements);
        return element;
    }

    /// <summary>
    /// Filters out all of the disabled <see cref="IElement"/>s in an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="elements">The elements to filter.</param>
    /// <returns>The filtered <see cref="IEnumerable{T}"/>.</returns>
    public static IEnumerable<IElement> FilterDisabled(this IEnumerable<IElement> elements) => elements.Where(x => x.Enabled);

    /// <summary>
    /// Gets the functional (un-scaled) position of an element.
    /// </summary>
    /// <param name="element">The element to get the position for.</param>
    /// <returns>The un-scaled position..</returns>
    public static float GetFunctionalPosition(this IElement element) => Ruetility.ScaledPositionToFunctional(element.Position);
}