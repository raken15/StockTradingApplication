using StockTradingApplication.Models;
using StockTradingApplication.Repositories;
using StockTradingApplication.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;

namespace StockTradingApplication.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Fields
        private readonly IRepository<StockModel, string> _stockRepository;
        private StockViewModel _selectedStock;
        private FinancialPortfolioViewModel _financialPortfolio;
        private StockViewModel _selectedPortfolioStock;
        private DispatcherTimer _timer;
        #endregion
        #region Properties
        public ObservableCollection<StockViewModel> Stocks { get; set; }
        
        public StockViewModel SelectedStock
        {
            get => _selectedStock;
            set
            {
                if (_selectedStock != value)
                {
                    _selectedStock = value;
                    BuyStockCommand.RaiseCanExecuteChanged();
                    if (value != null)
                    {
                        SelectedPortfolioStock = null; // Clear SelectedPortfolioStock after SelectedStock is set
                    }
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
                    if (value != null)
                    {
                        SelectedStock = null; // Clear SelectedStock after SelectedPortfolioStock is set
                    }
                }
            }
        }
        #endregion
        #region Commands
        public RelayCommand BuyStockCommand { get; }
        public RelayCommand SellStockCommand { get; }
        public RelayCommand RestartCommand { get; }
        #endregion
        #region Constructor and initialization
        public MainViewModel()
        {
            _stockRepository = new StockModelRepository();
            Stocks = new ObservableCollection<StockViewModel>();
            InitializeStocks();

            var financialPortfolioModel = new FinancialPortfolioModel
            {
                Money = 1000.0f,
                Stocks = new List<StockModel>()
            };
            FinancialPortfolio = new FinancialPortfolioViewModel(financialPortfolioModel);

            BuyStockCommand = new RelayCommand(async (param) => await BuyStockAsync(), (param) => CanBuyStock());
            SellStockCommand = new RelayCommand(async (param) => await SellStockAsync(), (param) => CanSellStock());
            RestartCommand = new RelayCommand(Restart);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(0.3);
            _timer.Tick += UpdateStockPrices;
            _timer.Start();

            FinancialPortfolio.PropertyChanged += FinancialPortfolio_PropertyChanged;
        }
        private void InitializeStocks()
        {
            var stocks = _stockRepository.GetAll();
            foreach (var stock in stocks)
            {
                Stocks.Add(new StockViewModel(stock));
            }
        }
        #endregion
        #region Methods
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
        #endregion
        #region Timer event handler
        private void UpdateStockPrices(object sender, EventArgs e)
        {
            var random = new Random();
            var stocks = Stocks.ToList();
            var portfolioStocks = FinancialPortfolio.StocksPortfolio.ToList();

            foreach (var stock in stocks)
            {
                var newPrice = random.Next(1,4001) + random.Next(100) / 100.0f;
                stock.Price = newPrice;

                var portfolioStock = portfolioStocks.FirstOrDefault(ps => ps.Symbol == stock.Symbol);
                if (portfolioStock != null)
                {
                    portfolioStock.Price = newPrice;
                }
            }

            RaisePropertyChanged(nameof(Stocks));
            RaisePropertyChanged(nameof(FinancialPortfolio));
        }
        #endregion
        #region PropertyChanged event handler
         public event PropertyChangedEventHandler PropertyChanged;
         private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region FinancialPortfolioPropertyChanged event handler
        private void FinancialPortfolio_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FinancialPortfolioViewModel.Money))
            {
                if (FinancialPortfolio.Money >= 10000)
                {
                    MessageBox.Show("You win! you can continue if you wish :)", "Congratulations!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (FinancialPortfolio.Money <= 0)
                {
                    MessageBox.Show("You Lose! try not to let your money be 0 or less", "Game Over!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
        #region Restart event handler
        private void Restart(object obj)
        {
            var financialPortfolioModel = new FinancialPortfolioModel
            {
                Money = 1000.0f,
                Stocks = new List<StockModel>()
            };
            FinancialPortfolio = new FinancialPortfolioViewModel(financialPortfolioModel);

            Stocks.Clear();
            InitializeStocks();

            _timer.Start();
        }
        #endregion
    }
}