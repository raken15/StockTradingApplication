using StockTradingApplication.Models;

namespace StockTradingApplication.Repositories;

public class StockModelRepository : IRepository<StockModel, string>
{
    private readonly List<StockModel> _stocks;
    public enum PriceCondition
    {
        Above,
        Below,
        Equal
    }
    public int Count => _stocks.Count;
    public StockModelRepository()
    {
        _stocks = new List<StockModel>
        {
            new StockModel { Symbol = "AAPL", Quantity = 10, Price = 150.00f },
            new StockModel { Symbol = "GOOGL", Quantity = 5, Price = 2800.00f },
            new StockModel { Symbol = "MSFT", Quantity = 15, Price = 300.00f },
            new StockModel { Symbol = "AMZN", Quantity = 20, Price = 3000.00f },
            new StockModel { Symbol = "TSLA", Quantity = 5, Price = 500.00f }
        };
    }
    public StockModel Get(string stockSymbol)
    {
        var stock = _stocks.FirstOrDefault(s => s.Symbol == stockSymbol);
        if (stock == null)
        {
            System.Windows.MessageBox.Show($"Stock {stockSymbol} does not exist in the repository.", "Error");
        }
        return stock;
    }
    public bool TryGet(string stockSymbol, out StockModel stock)
    {
        stock = _stocks.FirstOrDefault(s => s.Symbol == stockSymbol);
        return stock != null;
    }
    public IEnumerable<StockModel> GetAll()
    {
        if (_stocks.Count == 0)
        {
            System.Windows.MessageBox.Show("No stocks are currently in the repository.", "Error");
        }
        return _stocks;
    }
    /// <summary>
    /// Updates or inserts a stock into the repository based on its symbol.
    /// </summary>
    public void Upsert(StockModel stock)
    {
        var existingStock = _stocks.FirstOrDefault(s => s.Symbol == stock.Symbol);
        if (existingStock != null)
        {
            existingStock.Quantity = stock.Quantity;
            existingStock.Price = stock.Price;
        }
        else
        {
            _stocks.Add(stock);
        }
    }
    public void Remove(string stockSymbol)
    {
        var stockToRemove = _stocks.FirstOrDefault(s => s.Symbol == stockSymbol);
        if (stockToRemove != null)
        {
            _stocks.Remove(stockToRemove);
        }
        else
        {
            System.Windows.MessageBox.Show($"Stock {stockSymbol} does not exist in the repository.", "Error");
        }
    }
    public bool TryRemove(string stockSymbol)
    {
        if(_stocks.Count > 0 && _stocks.Any(s => s.Symbol == stockSymbol))
        {
            _stocks.Remove(_stocks.FirstOrDefault(s => s.Symbol == stockSymbol));
            return true;
        }
        return false;
    }

    public void Clear()
    {
        _stocks.Clear();
    }
    public bool Contains(string stockSymbol)
    {
        return _stocks.Any(s => s.Symbol == stockSymbol);
    }
    public IEnumerable<StockModel> GetAllStocksByPriceCondition(float stockPrice, PriceCondition condition)
    {
        switch (condition)
        {
            case PriceCondition.Above:
                return _stocks.Where(x => x.Price > stockPrice);
            case PriceCondition.Below:
                return _stocks.Where(x => x.Price < stockPrice);
            case PriceCondition.Equal:
                return _stocks.Where(x => x.Price == stockPrice);
            default:
                throw new ArgumentOutOfRangeException(nameof(condition), condition, null);
        }
    }
}
