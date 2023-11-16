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
    /// <param name="element">The element to add.</param>
    /// <param name="container">The <see cref="IElementContainer"/> to add to.</param>
    /// <returns>A reference to this element.</returns>
    public static IElement AddTo(this IElement element, IElementContainer container) => element.AddTo(container.Elements);

    /// <summary>
    /// Gets the functional (un-scaled) position of an element.
    /// </summary>
    /// <param name="element">The element to get the position for.</param>
    /// <returns>The un-scaled position..</returns>
    public static float GetFunctionalPosition(this IElement element) => Ruetility.ScaledPositionToFunctional(element.Position);

    /// <summary>
    /// Calculates the offset for two hints.
    /// </summary>
    /// <param name="hintOnePos">The first hint's vertical position.</param>
    /// <param name="hintOneTotalLines">The first hint's total line-height, excluding the vertical position.</param>
    /// <param name="hintTwoPos">The second hint's vertical position.</param>
    /// <returns>A float indicating the new offset.</returns>
    public static float CalculateOffset(float hintOnePos, float hintOneTotalLines, float hintTwoPos)
    {
        float calc = (hintOnePos + (2 * hintOneTotalLines)) - hintTwoPos;
        return calc / -2;
    }
}