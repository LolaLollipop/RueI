namespace RueI;

/// <summary>
/// Represents a reference to an element present within any number of player's displays.
/// </summary>
/// <typeparam name="T">The type of the element to act as a reference to.</typeparam>
public sealed class ElemReference<T>
    where T : IElement
{
    /// <summary>
    /// Safely casts this to a <see cref="ElemReference{T}"/> with <see cref="IElement"/> generic.
    /// </summary>
    /// <param name="toCast">The <see cref="ElemReference{T}"/> to cast.</param>
    public static implicit operator ElemReference<IElement>(ElemReference<T> toCast) => (toCast as ElemReference<IElement>) !;
}
