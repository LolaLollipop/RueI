namespace RueI.Extensions;

/// <summary>
/// Represents an intersection between a class and an interface.
/// </summary>
/// <typeparam name="T">The more derived class.</typeparam>
/// <typeparam name="TOther">The interface to be derived from.</typeparam>
public class Intersection<T, TOther>
    where T : class
    where TOther : class
{
    private Intersection(T value)
    {
        Class = value;
        Interface = (value as TOther) !;
    }

    /// <summary>
    /// Gets the derived class of the intersection.
    /// </summary>
    public T Class { get; }

    /// <summary>
    /// Gets the interface of the intersection.
    /// </summary>
    public TOther Interface { get; }

    /// <summary>
    /// Implicitly casts an intersection to <typeparamref name="T"/>.
    /// </summary>
    /// <param name="intersection">The intersection to cast.</param>
    public static implicit operator T(Intersection<T, TOther> intersection) => intersection.Class;

    /// <summary>
    /// Implicitly casts an intersection to <typeparamref name="TOther"/>.
    /// </summary>
    /// <param name="intersection">The intersection to cast.</param>
    public static implicit operator TOther(Intersection<T, TOther> intersection) => intersection.Interface;

    /// <summary>
    /// Creates a new intersection for a class.
    /// </summary>
    /// <typeparam name="TClass">The class to create the intersection for.</typeparam>
    /// <param name="value">The value of the intersected type.</param>
    /// <returns>A new intersection of the two types.</returns>
    public static Intersection<T, TOther> New<TClass>(TClass value)
        where TClass : class, T, TOther
    {
        return new Intersection<T, TOther>(value);
    }
}
