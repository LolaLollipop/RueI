namespace RueI.Extensions
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides extensions for working with all types.
    /// </summary>
    public static class UniversalExtensions
    {
        /// <summary>
        /// Adds this instance to an <see cref="ICollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of this instance and the collection to add to.</typeparam>
        /// <param name="item">The instance to add.</param>
        /// <param name="collection">The collection to add the elements to.</param>
        public static void AddTo<T>(this T item, ICollection<T> collection)
        {
            collection.Add(item);
        }
    }
}
