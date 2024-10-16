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
        private FinancialPortfolioViewModel _financialPortfolio;
        private StockViewModel _selectedPortfolioStock;
        public ObservableCollection<StockViewModel> Stocks { get; set; }
        public RelayCommand BuyStockCommand { get; }
        public RelayCommand SellStockCommand { get; }
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
        public FinancialPortfolioViewModel FinancialPortfolio
        {
            get { return _financialPortfolio; }
            set
            {
                if (_financialPortfolio != value)
                {
                    _financialPortfolio = value;
                    RaisePropertyChanged(nameof(FinancialPortfolio));
                }
            }
        }
        public StockViewModel SelectedPortfolioStock
        {
            get => _selectedPortfolioStock;
            set
            {
                if (_selectedPortfolioStock != value)
                {
                    _selectedPortfolioStock = value;
                    SellStockCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public MainViewModel()
        {
            _stockRepository = new StockModelRepository();
            Stocks = new ObservableCollection<StockViewModel>();
            InitializeStocks();

            var financialPortfolioModel = new FinancialPortfolioModel
            {
                Money = 1000.0m,
                Stocks = new List<StockModel>()
            };
            FinancialPortfolio = new FinancialPortfolioViewModel(financialPortfolioModel);

            BuyStockCommand = new RelayCommand(async (param) => await BuyStockAsync(), (param) => CanBuyStock());
            SellStockCommand = new RelayCommand(async (param) => await SellStockAsync(), (param) => CanSellStock());
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
                // Simulated stock buying logic
                SelectedStock.Quantity--;

                await Task.Yield(); // Yield to the UI thread
                if(FinancialPortfolio.StocksPortfolio.Any(x => x.Symbol == SelectedStock.Symbol))
                {
                    FinancialPortfolio.StocksPortfolio.First(x => x.Symbol == SelectedStock.Symbol).Quantity++;
                }
                else
                {
                    FinancialPortfolio.StocksPortfolio.Add(new StockViewModel(
                    new StockModel() { Symbol = SelectedStock.Symbol,
                    Quantity = 1, Price = SelectedStock.Price }));
                }
                FinancialPortfolio.Money -= SelectedStock.Price;

                RaisePropertyChanged(nameof(Stocks));
                BuyStockCommand.RaiseCanExecuteChanged();
            }
        }
        private async Task SellStockAsync()
        {
            if (SelectedPortfolioStock != null)
            {
                await Task.Yield(); // Yield to the UI thread
                // Simulated stock buying logic
                SelectedPortfolioStock.Quantity--;
                FinancialPortfolio.Money += SelectedPortfolioStock.Price;
                if(Stocks.Any(x => x.Symbol == SelectedPortfolioStock.Symbol))
                {
                    Stocks.First(x => x.Symbol == SelectedPortfolioStock.Symbol).Quantity++;
                }
                //RaisePropertyChanged(nameof(Stocks));
                SellStockCommand.RaiseCanExecuteChanged();
            }
        }
        private bool CanBuyStock()
        {
            return SelectedStock != null && SelectedStock.Quantity > 0; // Can only buy if a stock is selected and has quantity
        }
        private bool CanSellStock()
        {
            return SelectedPortfolioStock != null && SelectedPortfolioStock.Quantity > 0; // Can only buy if a stock is selected and has quantity
        }

         public event PropertyChangedEventHandler PropertyChanged;
         private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}