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

    /// <summary>
    /// Determines if a <see cref="IEnumerable{T}"/> has only one element that passes a filter.
    /// </summary>
    /// <typeparam name="T">The inner type of the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to check.</param>
    /// <param name="filter">A filter to use.</param>
    /// <returns>true if there is only one element in the <see cref="IEnumerable{T}"/> and that element passes the filter, otherwise false.</returns>
    public static bool Only<T>(this IEnumerable<T> enumerable, Func<T, bool> filter)
    {
        using IEnumerator<T> enumerator = enumerable.GetEnumerator();
        return !enumerator.MoveNext() || !filter(enumerator.Current) || enumerator.MoveNext();
    }

    /// <summary>
    /// Gets a <see cref="IEnumerable{T}"/> of values from a <see cref="IDictionary{TKey, TValue}"/> using the key.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary's key.</typeparam>
    /// <typeparam name="TValue">The type of the dictionary's value.</typeparam>
    /// <param name="dict">The dictionary to use.</param>
    /// <param name="key">The key to search with.</param>
    /// <returns>The <see cref="IEnumerable{T}"/>, or <see cref="Enumerable.Empty{TResult}"/> if it is not found.</returns>
    public static IEnumerable<TValue> GetMultiple<TKey, TValue>(this IDictionary<TKey, IEnumerable<TValue>> dict, TKey key)
    {
        return dict.TryGetValue(key, out IEnumerable<TValue> value) ? value : Enumerable.Empty<TValue>();
    }
}
