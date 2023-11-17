namespace RueI.Extensions;

using NorthwoodLib.Pools;

/// <summary>
/// Provides extensions for working with collections.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Converts a <see cref="IEnumerable{T}"/> to a pooled <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item for the list.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to convert.</param>
    /// <returns>The new pooled <see cref="List{T}"/>.</returns>
    public static List<T> ToPooledList<T>(this IEnumerable<T> enumerable)
    {
        List<T> pooledList = ListPool<T>.Shared.Rent();

        foreach (T item in enumerable)
        {
            pooledList.Add(item);
        }

        return pooledList;
    }

    /// <summary>
    /// Converts a <see cref="IEnumerable{T}"/> to a pooled <see cref="List{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item for the list.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to convert.</param>
    /// <param name="capacity">The initial capacity of the <see cref="List{T}"/>.</param>
    /// <returns>The new pooled <see cref="List{T}"/>.</returns>
    public static List<T> ToPooledList<T>(this IEnumerable<T> enumerable, int capacity)
    {
        List<T> pooledList = ListPool<T>.Shared.Rent(capacity);

        foreach (T item in enumerable)
        {
            pooledList.Add(item);
        }

        return pooledList;
    }
}
