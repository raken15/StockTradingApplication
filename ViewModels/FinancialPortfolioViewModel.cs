using System;
using System.ComponentModel;
using StockTradingApplication.Models;

namespace StockTradingApplication.ViewModels;

public class FinancialPortfolioViewModel : INotifyPropertyChanged
{
    private FinancialPortfolioModel _model;
    public List<StockViewModel> StocksPorfolio { get; set; }
    private decimal _money; 
    public decimal Money
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
        StocksPorfolio = _model.Stocks.Select(s => new StockViewModel(s)).ToList();
        _money = _model.Money;
    }
    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
