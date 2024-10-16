using System;
using StockTradingApplication.Models;

namespace StockTradingApplication.Repositories;

public class StockModelRepository : IRepository<StockModel, string>
{
    private readonly List<StockModel> _stocks = new List<StockModel>
    {
        new StockModel { Symbol = "AAPL", Quantity = 10, Price = 150.00m },
        new StockModel { Symbol = "GOOGL", Quantity = 5, Price = 2800.00m },
        new StockModel { Symbol = "MSFT", Quantity = 15, Price = 300.00m },
        new StockModel { Symbol = "AMZN", Quantity = 20, Price = 3000.00m },
        new StockModel { Symbol = "TSLA", Quantity = 5, Price = 500.00m }
    };

    public StockModel Get(string stockSymbol)
    {
        var stock = _stocks.FirstOrDefault(s => s.Symbol == stockSymbol);
        if (stock == null)
        {
            System.Windows.MessageBox.Show($"Stock {stockSymbol} does not exist in the repository.", "Error");
        }
        return stock;
    }
    public IEnumerable<StockModel> GetAll()
    {
        if (_stocks.Count == 0)
        {
            System.Windows.MessageBox.Show("No stocks are currently in the repository.", "Error");
            return Enumerable.Empty<StockModel>();
        }
        return _stocks;
    }

    public void Add(StockModel stock)
    {
        if (_stocks.Any(x => x.Symbol == stock.Symbol))
        {
            System.Windows.MessageBox.Show($"Stock {stock.Symbol} already exists in the repository.", "Error");
            return;
        }
        _stocks.Add(stock);
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
    public IEnumerable<StockModel> GetAllBelowPrice(decimal stockPrice)
    {
        if (_stocks.Count == 0)
        {
            System.Windows.MessageBox.Show("No stocks are currently in the repository.", "Error");
            return Enumerable.Empty<StockModel>();
        }
        return _stocks.Where(x => x.Price < stockPrice);
    }
    public IEnumerable<StockModel> GetAllAbovePrice(decimal stockPrice)
    {
        if (_stocks.Count == 0)
        {
            System.Windows.MessageBox.Show("No stocks are currently in the repository.", "Error");
            return Enumerable.Empty<StockModel>();
        }
        return _stocks.Where(x => x.Price > stockPrice);
    }
}
