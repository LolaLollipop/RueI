namespace RueI.Extensions;

/// <summary>
/// Represents a union between a class and an interface.
/// </summary>
/// <typeparam name="T">The more derived class.</typeparam>
/// <typeparam name="TOther">The interface to be derived from.</typeparam>
public class Union<T, TOther>
    where T : class
    where TOther : class
{
    private Union(T value)
    {
        Class = value;
        Interface = (value as TOther) !;
    }

    /// <summary>
    /// Gets the derived class of the union.
    /// </summary>
    public T Class { get; }

    /// <summary>
    /// Gets the interface of the union.
    /// </summary>
    public TOther Interface { get; }

    public static implicit operator T(Union<T, TOther> union) => union.Class;

    public static implicit operator TOther(Union<T, TOther> union) => union.Interface;

    /// <summary>
    /// Creates a new union for a class.
    /// </summary>
    /// <typeparam name="TClass">The class to create the union for.</typeparam>
    /// <param name="value">The value of the unioned type.</param>
    /// <returns>A new union of the two types.</returns>
    public static Union<T, TOther> New<TClass>(TClass value)
        where TClass : class, T, TOther
    {
        return new Union<T, TOther>(value);
    }
}
