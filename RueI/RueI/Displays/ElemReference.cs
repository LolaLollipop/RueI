namespace RueI.Displays;

using RueI.Elements;

/// <summary>
/// Represents a reference to an element present within any number of player's displays.
/// </summary>
/// <typeparam name="T">The type of the element to act as a reference to.</typeparam>
public readonly struct ElemReference<T> : IEquatable<ElemReference<T>>
    where T : IElement
{
    private static int nextID = 0;

    private readonly int id;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElemReference{T}"/> struct.
    /// </summary>
    public ElemReference()
    {
        id = nextID;
        nextID++;
    }

    private ElemReference(int id)
    {
        this.id = id;
    }

    /// <summary>
    /// Safely casts this to a <see cref="ElemReference{T}"/> with <see cref="IElement"/> generic.
    /// </summary>
    /// <param name="toCast">The <see cref="ElemReference{T}"/> to cast.</param>
    public static implicit operator ElemReference<IElement>(ElemReference<T> toCast) => new(toCast.id);

    /// <summary>
    /// Compares this <see cref="ElemReference{T}"/> to another <see cref="ElemReference{T}"/>.
    /// </summary>
    /// <param name="other">The other <see cref="ElemReference{T}"/>.</param>
    /// <returns>A value indicating whether or not the two are equal.</returns>
    public readonly bool Equals(ElemReference<T> other) => id == other.id;
}
