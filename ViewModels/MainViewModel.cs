using StockTradingApplication.Models;
using StockTradingApplication.Repositories;
using StockTradingApplication.Helpers;
using System.Collections.ObjectModel;

namespace StockTradingApplication.ViewModels
{
    public class MainViewModel
    {
        private readonly IRepository<Stock,string> _stockRepository;
        public ObservableCollection<Stock> Stocks { get; set; }
        public RelayCommand BuyStockCommand { get; }

        private Stock _selectedStock;

        public Stock SelectedStock
        {
            get => _selectedStock;
            set
            {
                if (_selectedStock != value)
                {
                    _selectedStock = value;
                    BuyStockCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public MainViewModel()
        {
            _stockRepository = new StockRepository();
            Stocks = new ObservableCollection<Stock>(_stockRepository.GetAll());

            BuyStockCommand = new RelayCommand(async (param) => await BuyStockAsync(), (param) => CanBuyStock());
        }
        private async Task BuyStockAsync()
        {
            if (SelectedStock != null)
            {
                await Task.Run(() => 
                {
                    // Simulated stock buying logic
                    SelectedStock.Quantity--;
                    //StockBought?.Invoke(SelectedStock); // Raise event after buying stock
                });
            }
            BuyStockCommand.RaiseCanExecuteChanged();
        }
        private bool CanBuyStock()
        {
            return SelectedStock != null && SelectedStock.Quantity > 0; // Can only buy if a stock is selected and has quantity
        }
    }
}