using System.ComponentModel;
using StockTradingApplication.Models;

namespace StockTradingApplication.ViewModels;

/// <summary>
/// A view model for a stock, provides properties for binding to the stock's symbol, quantity and price.
/// The view model implements INotifyPropertyChanged and raises PropertyChanged events
/// when the properties are changed. The view model also provides a constructor to create
/// a view model from a StockModel. The view model can be used to bind to a stock's data
/// in a user interface.
/// </summary>
public class StockViewModel : INotifyPropertyChanged
{
    private StockModel _stock;

    public StockViewModel(StockModel stock)
    {
        _stock = stock;
    }
    public string Symbol
    {
        get => _stock.Symbol;
        set
        {
            if (_stock.Symbol != value)
            {
                _stock.Symbol = value;
                RaisePropertyChanged(nameof(Symbol));
            }
        }
    }
    public int Quantity
    {
        get => _stock.Quantity;
        set
        {
            if (_stock.Quantity != value)
            {
                _stock.Quantity = value;
                RaisePropertyChanged(nameof(Quantity));
            }
        }
    }
    public float Price
    {
        get => _stock.Price;
        set
        {
            if (_stock.Price != value)
            {
                _stock.Price = value;
                RaisePropertyChanged(nameof(Price));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

