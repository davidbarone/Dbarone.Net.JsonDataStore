using System;

namespace Dbarone.Net.JsonDataStore
{
    /// <summary>
    /// A collection of items.
    /// </summary>
    /// <typeparam name="T">The element type for the collection.</typeparam>
    public interface IDocumentCollection<T>
    {
        /// <summary>
        /// Find all items matching a query.
        /// </summary>
        /// <param name="where">The filter predicate.</param>
        /// <returns>Returns items matching the predicate.</returns>
        IEnumerable<T> Find(Predicate<T> where);

        /// <summary>
        /// Full-text search.
        /// </summary>
        /// <param name="text">Search text</param>
        /// <param name="caseSensitive">Is the search case sensitive</param>
        /// <returns>Items matching the search text</returns>
        IEnumerable<T> FullTextSearch(string text, bool caseSensitive = false);

        /// <summary>
        /// Gets the next id value.
        /// </summary>
        /// <returns>Returns an integer value.</returns>
        int GetNextId();

        /// <summary>
        /// Inserts an item to the collection.
        /// </summary>
        /// <param name="item">The new item to be inserted.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        int Insert(T item);

        /// <summary>
        /// Inserts multiple items to the collection.
        /// </summary>
        /// <param name="items">The new items to be inserted.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        int Insert(IEnumerable<T> items);

        /// <summary>
        /// Updates an item in the collection, or inserts a new value.
        /// </summary>
        /// <param name="where">The filter predicate.</param>
        /// <param name="item">The new item to upsert.</param>
        /// <returns>Returns true if an item is found. Returns false if new value is inserted.</returns>
        int Upsert(Predicate<T> where, T item);

        /// <summary>
        /// Updates a collection with an item.
        /// </summary>
        /// <param name="where">The filter predicate.</param>
        /// <param name="item">The new item to update.</param>
        /// <returns>Returns true if an item is found to update. Does not update / insert if no item found.</returns>
        int Update(Predicate<T> where, Func<T, T> transform);

        /// <summary>
        /// Deletes items from a collection.
        /// </summary>
        /// <param name="where">The filter predicate.</param>
        /// <returns>Returns true if any items deleted.</returns>
        int Delete(Predicate<T> where);

        /// <summary>
        /// Returns the number of items in the collection.
        /// </summary>
        int Count { get; }
    }
}