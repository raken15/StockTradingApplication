namespace StockTradingApplication.Repositories
{
    public interface IRepository<T, K> where T : notnull where K : notnull
    {
        T Get(K key);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Remove(K key);
        void RemoveAll();
    }
}