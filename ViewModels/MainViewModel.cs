using StockTradingApplication.Models;
using StockTradingApplication.Repositories;
using StockTradingApplication.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StockTradingApplication.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<StockModel, string> _stockRepository;
        private StockViewModel _selectedStock;
        public ObservableCollection<StockViewModel> Stocks { get; set; }
        public RelayCommand BuyStockCommand { get; }
        public StockViewModel SelectedStock
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
            _stockRepository = new StockModelRepository();
            Stocks = new ObservableCollection<StockViewModel>();
            InitializeStocks();

            BuyStockCommand = new RelayCommand(async (param) => await BuyStockAsync(), (param) => CanBuyStock());
        }
        private void InitializeStocks()
        {
            var stocks = _stockRepository.GetAll();
            foreach (var stock in stocks)
            {
                Stocks.Add(new StockViewModel(stock));
            }
        }
        private async Task BuyStockAsync()
        {
            if (SelectedStock != null)
            {
                await Task.Run(() => 
                {
                    // Simulated stock buying logic
                    SelectedStock.Quantity--;
                    RaisePropertyChanged(nameof(Stocks));
                });
            }
            BuyStockCommand.RaiseCanExecuteChanged();
        }
        private bool CanBuyStock()
        {
            return SelectedStock != null && SelectedStock.Quantity > 0; // Can only buy if a stock is selected and has quantity
        }
         public event PropertyChangedEventHandler PropertyChanged;
         private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}