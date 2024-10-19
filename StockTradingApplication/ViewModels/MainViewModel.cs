using StockTradingApplication.Models;
using StockTradingApplication.Repositories;
using StockTradingApplication.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;
using System.Runtime.Serialization;

namespace StockTradingApplication.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Constants
        private const float STARTING_MONEY = 1000.0f;
        private const float WINNING_MONEY = 10000.0f;
        private const float LOSING_MONEY = 1.0f;
        private const int HIGHEST_STOCK_PRICE = 4000;
        private const int LOWEST_STOCK_PRICE = 1;
        private const int TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS = 1;  // 1 second
        private const int TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS = 60;  // 1 minute

        #endregion
        #region Fields
        private IRepository<StockModel, string> _stockRepository;
        private StockViewModel _selectedStock;
        private FinancialPortfolioViewModel _financialPortfolio;
        private StockViewModel _selectedPortfolioStock;
        private DispatcherTimer _timerUpdatePrices;
        private TimeSpan _elapsedTime;
        private DispatcherTimer _elapsedTimeTimer;
        private string _message;
        private bool _isMessageVisible;
        private TimeSpan _remainingTimeBeforeNextPriceUpdate;
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
        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                if (_elapsedTime != value)
                {
                    _elapsedTime = value;
                    RaisePropertyChanged(nameof(ElapsedTime));
                }
            }
        }
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged(nameof(Message));
                }
            }
        }

        public bool IsMessageVisible
        {
            get { return _isMessageVisible; }
            set
            {
                if (_isMessageVisible != value)
                {
                    _isMessageVisible = value;
                    RaisePropertyChanged(nameof(IsMessageVisible));
                }
            }
        }
        public TimeSpan RemainingTimeBeforeNextPriceUpdate
        {
            get => _remainingTimeBeforeNextPriceUpdate;
            set
            {
                if (_remainingTimeBeforeNextPriceUpdate != value)
                {
                    if (_remainingTimeBeforeNextPriceUpdate == TimeSpan.Zero)
                    {
                        _remainingTimeBeforeNextPriceUpdate = TimeSpan.FromSeconds(TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS);
                    }
                    else
                    {
                        _remainingTimeBeforeNextPriceUpdate = value;
                    }
                    RaisePropertyChanged(nameof(RemainingTimeBeforeNextPriceUpdate));
                }
            }
        }
        #endregion
        #region Commands
        public RelayCommand BuyStockCommand { get; set; }
        public RelayCommand SellStockCommand { get; set; }
        public RelayCommand RestartCommand { get; set; }
        public RelayCommand CloseMessageCommand { get; set; }
        #endregion
        #region Constructor and initialization
        public MainViewModel()
        {
            InitializeStockRepository();
            InitializeFinancialPortfolio();
            InitializeCommands();
            InitializeTimers();
            SimulateTick(); // make the initial stocks to start with random prices
        }
        private void InitializeStockRepository()
        {
            _stockRepository = new StockModelRepository();
            InitializeStocks();
        }
        private void InitializeStocks()
        {
            if (Stocks == null)
            {
                Stocks = new ObservableCollection<StockViewModel>();
            }
            else
            {
                Stocks.Clear();
            }
            var stocks = _stockRepository.GetAll();
            foreach (var stock in stocks)
            {
                Stocks.Add(new StockViewModel(stock));
            }
        }
        private void InitializeFinancialPortfolio()
        {
            if (FinancialPortfolio == null)
            {
                var financialPortfolioModel = new FinancialPortfolioModel
                {
                    Money = STARTING_MONEY,
                    Stocks = new List<StockModel>()
                };
                FinancialPortfolio = new FinancialPortfolioViewModel(financialPortfolioModel);
                FinancialPortfolio.PropertyChanged += FinancialPortfolio_PropertyChanged;
            }
            else
            {
                FinancialPortfolio.StocksPortfolio.Clear();
                FinancialPortfolio.Money = STARTING_MONEY;
            }
        }
        private void InitializeCommands()
        {
            BuyStockCommand = new RelayCommand((param) => BuyStock(), (param) => CanBuyStock());
            SellStockCommand = new RelayCommand((param) => SellStock(), (param) => CanSellStock());
            RestartCommand = new RelayCommand(Restart);
            CloseMessageCommand = new RelayCommand(CloseMessage);
        }
        private void InitializeTimers()
        {
            ElapsedTime = default(TimeSpan);
            RemainingTimeBeforeNextPriceUpdate = TimeSpan.FromSeconds(TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS);
            if (_timerUpdatePrices != null && _elapsedTimeTimer != null)
            {
                _timerUpdatePrices.Start();
                _elapsedTimeTimer.Start();
            }
            else
            {
                _timerUpdatePrices = new DispatcherTimer
                { Interval = TimeSpan.FromSeconds(TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS) };
                _timerUpdatePrices.Tick += UpdateStockPrices;
                _timerUpdatePrices.Start();
                _elapsedTimeTimer = new DispatcherTimer
                { Interval = TimeSpan.FromSeconds(TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS) };
                _elapsedTimeTimer.Tick += (sender, args) =>
                {
                    ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS));
                    RemainingTimeBeforeNextPriceUpdate = TimeSpan.FromSeconds(TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS - ElapsedTime.Seconds);
                };
                _elapsedTimeTimer.Start();
            }
        }
        #endregion
        #region Methods
        private void BuyStock()
        {
            if (SelectedStock != null)
            {
                // Simulated stock buying logic
                SelectedStock.Quantity--;

                if (FinancialPortfolio.StocksPortfolio.Any(x => x.Symbol == SelectedStock.Symbol))
                {
                    FinancialPortfolio.StocksPortfolio.First(x => x.Symbol == SelectedStock.Symbol).Quantity++;
                }
                else
                {
                    FinancialPortfolio.StocksPortfolio.Add(new StockViewModel(
                    new StockModel()
                    {
                        Symbol = SelectedStock.Symbol,
                        Quantity = 1,
                        Price = SelectedStock.Price
                    }));
                }
                FinancialPortfolio.Money -= SelectedStock.Price;

                RaisePropertyChanged(nameof(Stocks));
                BuyStockCommand.RaiseCanExecuteChanged();
            }
        }
        private void SellStock()
        {
            if (SelectedPortfolioStock != null)
            {
                // Simulated stock selling logic
                SelectedPortfolioStock.Quantity--;
                FinancialPortfolio.Money += SelectedPortfolioStock.Price;
                if (Stocks.Any(x => x.Symbol == SelectedPortfolioStock.Symbol))
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
        private void ShowMessage(string message)
        {
            Message = message;
            IsMessageVisible = true;
        }
        private void StopTimers()
        {
            _timerUpdatePrices.Stop();
            _elapsedTimeTimer.Stop();
        }
        #endregion
        #region Event handlers
        #region Update stock prices handler
        private void UpdateStockPrices(object sender, EventArgs e)
        {
            var random = new Random();
            foreach (var stock in Stocks)
            {
                var newPrice = random.Next(LOWEST_STOCK_PRICE, HIGHEST_STOCK_PRICE + 1) + random.Next(100) / 100.0f;
                stock.Price = newPrice;

                var portfolioStock = FinancialPortfolio.StocksPortfolio.FirstOrDefault(ps => ps.Symbol == stock.Symbol);
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
                if (FinancialPortfolio.Money >= WINNING_MONEY)
                {
                    StopTimers();
                    ShowMessage("Congratulations! You win! you reached $10000, clicking ok will restart the game");
                }
                else if (FinancialPortfolio.Money < LOSING_MONEY)
                {
                    StopTimers();
                    ShowMessage("Game Over! You Lose! you have less than $1, you can try again if you wish, clicking ok will restart the game");
                }
            }
        }
        #endregion
        #region Restart event handler
        private void Restart(object obj)
        {
            InitializeFinancialPortfolio();
            InitializeStockRepository();
            InitializeTimers();
        }
        #endregion
        #region CloseMessage event handler
        private void CloseMessage(object obj)
        {
            Restart(null);
            IsMessageVisible = false;
        }
        #endregion
        #endregion
        #region Public Methods
        // Method to simulate timer tick
        public void SimulateTick()
        {
            // Trigger the tick event manually
            UpdateStockPrices(this, EventArgs.Empty);
        }
        #endregion

        #region Dispose and Cleanup

        public void Dispose()
        {
            if (_timerUpdatePrices != null)
            {
                _timerUpdatePrices.Stop();
                _timerUpdatePrices.Tick -= UpdateStockPrices;
                _timerUpdatePrices = null;
            }
            if (_elapsedTimeTimer != null)
            {
                _elapsedTimeTimer.Stop();
                _elapsedTimeTimer.Tick -= (s, e) => ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS));
                _elapsedTimeTimer = null;
            }

            if (Stocks != null)
            {
                Stocks.Clear();
            }

            if (FinancialPortfolio != null)
            {
                FinancialPortfolio.StocksPortfolio.Clear();
                FinancialPortfolio.PropertyChanged -= FinancialPortfolio_PropertyChanged;

            }
            if (_stockRepository != null)
            {
                _stockRepository.RemoveAll();
            }
        }
        #endregion
    }
}