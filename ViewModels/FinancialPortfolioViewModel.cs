using System;
using System.ComponentModel;
using StockTradingApplication.Models;
using System.Collections.ObjectModel;

namespace StockTradingApplication.ViewModels;

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
