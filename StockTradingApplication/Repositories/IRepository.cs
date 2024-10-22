namespace StockTradingApplication.Repositories
{
    /// <summary>
    /// A repository interface for generic CRUD operations. It is type-safe in both the entity and key types.
    /// </summary>
    public interface IRepository<T, TKey> where T : notnull where TKey : notnull
    {
        /// <summary>Gets the amount of items in the repository.</summary>
        int Count { get; }
        /// <summary>Gets the item with the given key.</summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The item with the given key. or default value if no such item exists.</returns>
        T Get(TKey key);

        /// <summary>
        /// Attempts to retrieve an item based on the provided key.
        /// </summary>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <param name="item">When this method returns, contains the item associated with the specified key, if the key is found; otherwise, the default value for the type of the item parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the item with the specified key is found; otherwise, false.
        /// </returns>
        bool TryGet(TKey key, out T item);

        /// <summary>Gets all items in the repository.</summary>
        /// <returns>An enumerable containing all items in the repository. Empty if the repository is empty.</returns>
        IEnumerable<T> GetAll();

        /// <summary>Inserts or updates an item in the repository.</summary>
        /// <param name="item">The item to insert or update.</param>
        void Upsert(T item);

        /// <summary>Removes the item with the specified key from the repository.</summary>
        /// <param name="key">The key of the item to remove.</param>
        void Remove(TKey key);

        /// <summary>
        /// Attempts to remove the item with the specified key from the repository.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>True if the item was successfully removed; otherwise, false.</returns>
        bool TryRemove(TKey key);

        /// <summary>Removes all items from the repository.</summary>
        void Clear();

        /// <summary>Checks if the repository contains the given key.</summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>True if the repository contains the item with the given key, false otherwise.</returns>
        bool Contains(TKey key);
    }
}