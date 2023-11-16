namespace RueI.Extensions
{
    /// <summary>
    /// Provides extensions for working with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds multiple items to a collection.
        /// </summary>
        /// <typeparam name="T">The type of item to add.</typeparam>
        /// <param name="collection">The collection to add the elements to.</param>
        /// <param name="items">The items to add.</param>
        public static void Add<T>(this ICollection<T> collection, params T[] items)
        {
            foreach (T item in items)
            {
                collection.Add(item);
            }
        }
    }
}
