using System.ComponentModel;
using StockTradingApplication.Models;
using System.Collections.ObjectModel;

namespace StockTradingApplication.ViewModels;

/// <summary>
/// The FinancialPortfolioViewModel class acts as a bridge between the UI and the FinancialPortfolioModel.
/// It provides a data binding mechanism for the UI to display and update financial portfolio information,
/// including the collection of stocks and available money. It implements INotifyPropertyChanged to notify
/// the UI of any property changes, enabling dynamic updates to the UI elements. The class initializes
/// the StocksPortfolio with data from the underlying model and allows property changes to be tracked
/// through the RaisePropertyChanged method.
/// </summary>
public class FinancialPortfolioViewModel : INotifyPropertyChanged
{
    private FinancialPortfolioModel _model;
    private float _money;
    private ObservableCollection<StockViewModel> _stocksPortfolio;
    public ObservableCollection<StockViewModel> StocksPortfolio
    {
        get { return _stocksPortfolio; }
        set
        {
            if (_stocksPortfolio != value)
            {
                _stocksPortfolio = value;
                RaisePropertyChanged(nameof(StocksPortfolio));
            }
        }
    }
    public float Money
    {
        get { return _money; }
        set
        {
            if (_money != value)
            {
                _money = value;
                RaisePropertyChanged(nameof(Money));
            }
        }
    }
    public FinancialPortfolioViewModel(FinancialPortfolioModel model)
    {
        _model = model;
        _stocksPortfolio = new ObservableCollection<StockViewModel>(_model.Stocks.Select(s => new StockViewModel(s)));
        _money = _model.Money;
    }
    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
