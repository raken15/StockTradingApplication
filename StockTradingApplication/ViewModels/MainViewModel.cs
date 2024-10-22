using StockTradingApplication.Models;
using StockTradingApplication.Repositories;
using StockTradingApplication.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;
using System.IO;

namespace StockTradingApplication.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Constants
        #endregion
        #region Fields
        private IRepository<StockModel, string> _stockRepository;
        private StockViewModel _selectedStock;
        private FinancialPortfolioViewModel _financialPortfolio;
        private StockViewModel _selectedPortfolioStock;
        private DispatcherTimer _timerUpdatePrices;
        private TimeSpan _elapsedTime;
        private DispatcherTimer _elapsedTimeTimer;
        private string _messageOverlayText;
        private bool _isMessageOverlayVisible;
        private TimeSpan _remainingTimeBeforeNextPriceUpdate;
        private PriceCondition _selectedPriceCondition;
        private float _priceConditionAmount;
        private ObservableCollection<PriceCondition> _priceConditionOptions;
        // Define the required settings and their expected types
        private readonly Dictionary<string, Type> _requiredSettings = new Dictionary<string, Type>
        {
            { "STARTING_MONEY", typeof(float) },
            { "WINNING_MONEY", typeof(float) },
            { "LOSING_MONEY", typeof(float) },
            { "HIGHEST_STOCK_PRICE", typeof(int) },
            { "LOWEST_STOCK_PRICE", typeof(int) },
            { "TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS", typeof(int) },
            { "TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS", typeof(int) },
            // Add more required settings here
        };
        private readonly string _initialSettingsFileName = "InitialSettings.txt";
        private readonly SimpleLogger _logger;
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
                    ((RelayCommand<object>)BuyStockCommand).RaiseCanExecuteChanged();
                    if (value != null)
                    {
                        SelectedPortfolioStock = null; // Clear SelectedPortfolioStock after SelectedStock is set
                    }
                    RaisePropertyChanged(nameof(SelectedStock));
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
                    ((RelayCommand<object>)SellStockCommand).RaiseCanExecuteChanged();
                    if (value != null)
                    {
                        SelectedStock = null; // Clear SelectedStock after SelectedPortfolioStock is set
                    }
                    RaisePropertyChanged(nameof(SelectedPortfolioStock));
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
        public string MessageOverlayText
        {
            get { return _messageOverlayText; }
            set
            {
                if (_messageOverlayText != value)
                {
                    _messageOverlayText = value;
                    RaisePropertyChanged(nameof(MessageOverlayText));
                }
            }
        }

        public bool IsMessageOverlayVisible
        {
            get { return _isMessageOverlayVisible; }
            set
            {
                if (_isMessageOverlayVisible != value)
                {
                    _isMessageOverlayVisible = value;
                    RaisePropertyChanged(nameof(IsMessageOverlayVisible));
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
                        _remainingTimeBeforeNextPriceUpdate = TimeSpan.FromSeconds(InitialSettingsDict["TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS"]);
                    }
                    else
                    {
                        _remainingTimeBeforeNextPriceUpdate = value;
                    }
                    RaisePropertyChanged(nameof(RemainingTimeBeforeNextPriceUpdate));
                }
            }
        }

        public PriceCondition SelectedPriceCondition
        {
            get => _selectedPriceCondition;
            set
            {
                _selectedPriceCondition = value;
                RaisePropertyChanged(nameof(SelectedPriceCondition));
                ((RelayCommand<object>)BuyStocksWithConditionCommand).RaiseCanExecuteChanged();
                ((RelayCommand<object>)SellStocksWithConditionCommand).RaiseCanExecuteChanged();
            }
        }
        public float PriceConditionAmount
        {
            get => _priceConditionAmount;
            set
            {
                if (_priceConditionAmount != value)
                {
                    _priceConditionAmount = value;
                    RaisePropertyChanged(nameof(PriceConditionAmount));
                    ((RelayCommand<object>)BuyStocksWithConditionCommand).RaiseCanExecuteChanged();
                    ((RelayCommand<object>)SellStocksWithConditionCommand).RaiseCanExecuteChanged();
                }
            }
        }
        public ObservableCollection<PriceCondition> PriceConditionOptions
        {
            get => _priceConditionOptions;
            set
            {
                _priceConditionOptions = value;
                RaisePropertyChanged(nameof(PriceConditionOptions));
            }
        }
        public Dictionary<string, dynamic> InitialSettingsDict { get; private set; }
        #endregion
        #region Commands
        public ICommand BuyStockCommand { get; private set; }
        public ICommand SellStockCommand { get; private set; }
        public ICommand RestartCommand { get; private set; }
        public ICommand CloseMessageOverlayCommand { get; private set; }
        public ICommand BuyStocksWithConditionCommand { get; private set; }
        public ICommand SellStocksWithConditionCommand { get; private set; }
        #endregion
        #region Enums
        public enum PriceCondition
        {
            Above,
            Below,
            Equal
        }
        public enum TradeStockOperation
        {
            BuyStock,
            SellStock
        }
        #endregion
        #region Constructor and initialization
        public MainViewModel()
        {
            _logger = new SimpleLogger("Logs/log.txt");
            _logger.LogInfo("MainViewModel created");
            if (!InitializeInitialSettings())
            {
                _logger.LogError("Failed to initialize initial settings.");
                Environment.Exit(1);
            }
            _stockRepository = new StockModelRepository();
            InitializeCommands();
            PopulatePriceConditions();
            InitializationOnRestart();
        }
        private void InitializationOnRestart()
        {
            InitializeStocks();
            InitializeFinancialPortfolio();
            InitializeTimers();
            SimulateTick(); // make the initial stocks to start with random prices
        }
        private bool InitializeInitialSettings()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", _initialSettingsFileName);
            InitialSettingsDict = LoadDictionaryFromFile(filePath);
            if (InitialSettingsDict == null || InitialSettingsDict.Count == 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Reads the settings from the text file and stores them in a dictionary.
        /// </summary>
        /// <param name="filePath">The path to the settings file.</param>
        /// <returns>A dictionary containing the key-value pairs.</returns>
        private Dictionary<string, dynamic> LoadDictionaryFromFile(string filePath)
        {
            var dictionaryFromFile = new Dictionary<string, dynamic>();
            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        // Skip empty or comment lines
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            continue;
                        // Split each line into key and value by colon
                        var parts = line.Split(':');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();   // Get the key and trim whitespace
                            string valueString = parts[1].Trim(); // Get the value and trim whitespace

                            if (!dictionaryFromFile.ContainsKey(key))
                            {
                                if (int.TryParse(valueString, out int intValue))
                                {
                                    dictionaryFromFile[key] = intValue;
                                }
                                else if (float.TryParse(valueString, out float floatValue))
                                {
                                    dictionaryFromFile[key] = floatValue;
                                }
                                else
                                {
                                    dictionaryFromFile[key] = valueString; // Fallback to storing as string
                                }
                            }
                            else
                            {
                                throw new Exception($"Duplicate key found: {key}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Invalid format in line: {line}");
                        }
                    }
                    if(dictionaryFromFile.Count == 0)
                    {
                        throw new Exception($"No key-value pairs found in file: {filePath}");
                    }
                    return ValidateRequiredKeys(dictionaryFromFile, _requiredSettings) ? dictionaryFromFile : null;
                }
                else
                {
                    throw new Exception($"File not found: {filePath}");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"Failed loading dictionary from file: {filePath}");
                return null;
            }
        }
        /// <summary>
        /// Validates that all required keys are present in the dictionary.
        /// </summary>
        /// <param name="dictionaryChecked">The dictionary containing the loaded settings.</param>
        /// <param name="requiredKeys">The list of required keys.</param>
        /// <returns>True if all required keys are present, false otherwise.</returns>
        private bool ValidateRequiredKeys(Dictionary<string, dynamic> dictionaryNeedsValidation, Dictionary<string, Type> requiredSettingsForValidation)
        {
            foreach (var requiredSetting in requiredSettingsForValidation)
            {
                string key = requiredSetting.Key;
                Type expectedType = requiredSetting.Value;
                if (!dictionaryNeedsValidation.ContainsKey(key))
                {
                    _logger.LogError($"Missing required setting: {key}");
                    return false; // Return false if a required key is missing
                }
                // Check if the value is of the expected type
                if (dictionaryNeedsValidation[key]?.GetType() != expectedType)
                {
                    _logger.LogError($"Invalid type for setting: {key}. Expected {expectedType}, but got {InitialSettingsDict[key]?.GetType()}");
                    return false; // Return false if the type doesn't match
                }
            }
            _logger.LogInfo("All required settings are present. Validation successful.");
            return true;
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
            var stocksFromRepository = _stockRepository.GetAll();
            foreach (var stock in stocksFromRepository)
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
                    Money = InitialSettingsDict["STARTING_MONEY"],
                    Stocks = new List<StockModel>()
                };
                FinancialPortfolio = new FinancialPortfolioViewModel(financialPortfolioModel);
                FinancialPortfolio.PropertyChanged += FinancialPortfolio_PropertyChanged;
            }
            else
            {
                FinancialPortfolio.StocksPortfolio.Clear();
                FinancialPortfolio.Money = InitialSettingsDict["STARTING_MONEY"];
            }
        }
        private void InitializeCommands()
        {
            if (BuyStockCommand == null)
            {
                BuyStockCommand = new RelayCommand<object>(_ => TradeStock(TradeStockOperation.BuyStock), _ => CanBuyOrSellStocks(TradeStockOperation.BuyStock));
            }
            if (SellStockCommand == null)
            {
                SellStockCommand = new RelayCommand<object>(_ => TradeStock(TradeStockOperation.SellStock), _ => CanBuyOrSellStocks(TradeStockOperation.SellStock));
            }
            if (RestartCommand == null)
            {
                RestartCommand = new RelayCommand<object>(_ => Restart());
            }
            if (CloseMessageOverlayCommand == null)
            {
                CloseMessageOverlayCommand = new RelayCommand<object>(_ => CloseMessageOverlay());
            }
            if (BuyStocksWithConditionCommand == null)
            {
                BuyStocksWithConditionCommand = new RelayCommand<object>(_ => BuyOrSellAllStocksByPriceCondition(TradeStockOperation.BuyStock), _ => CanBuyOrSellStocksWithCondition(TradeStockOperation.BuyStock));
            }
            if (SellStocksWithConditionCommand == null)
            {
                SellStocksWithConditionCommand = new RelayCommand<object>(_ => BuyOrSellAllStocksByPriceCondition(TradeStockOperation.SellStock), _ => CanBuyOrSellStocksWithCondition(TradeStockOperation.SellStock));
            }
        }
        private void InitializeTimers()
        {
            ElapsedTime = default(TimeSpan);
            RemainingTimeBeforeNextPriceUpdate = TimeSpan.FromSeconds(InitialSettingsDict["TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS"]);
            if (_timerUpdatePrices != null && _elapsedTimeTimer != null)
            {
                _timerUpdatePrices.Start();
                _elapsedTimeTimer.Start();
            }
            else
            {
                _timerUpdatePrices = new DispatcherTimer
                { Interval = TimeSpan.FromSeconds(InitialSettingsDict["TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS"]) };
                _timerUpdatePrices.Tick += UpdateStockPrices;
                _timerUpdatePrices.Start();
                _elapsedTimeTimer = new DispatcherTimer
                { Interval = TimeSpan.FromSeconds(InitialSettingsDict["TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS"]) };
                _elapsedTimeTimer.Tick += (sender, args) =>
                {
                    ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(InitialSettingsDict["TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS"]));
                    RemainingTimeBeforeNextPriceUpdate = TimeSpan.FromSeconds(InitialSettingsDict["TIMER_UPDATE_PRICES_INTERVAL_IN_SECONDS"] - ElapsedTime.Seconds);
                };
                _elapsedTimeTimer.Start();
            }
        }
        private void PopulatePriceConditions()
        {
            if (PriceConditionOptions == null)
            {
                PriceConditionOptions = new ObservableCollection<PriceCondition>();
                foreach (var condition in Enum.GetValues(typeof(PriceCondition)).Cast<PriceCondition>())
                {
                    PriceConditionOptions.Add(condition);
                }
            }
        }
        #endregion
        #region Methods
        private void TradeStock(TradeStockOperation tradeStockOperation)
        {
            if (tradeStockOperation == TradeStockOperation.BuyStock && SelectedStock != null)
            {
                // Simulated stock buying logic
                SelectedStock.Quantity--;
                UpdateFinancialPortfolioAfterBuyOrSell(SelectedStock, tradeStockOperation);
            }
            else if (tradeStockOperation == TradeStockOperation.SellStock && SelectedPortfolioStock != null)
            {
                // Simulated stock selling logic
                SelectedPortfolioStock.Quantity--;
                UpdateFinancialPortfolioAfterBuyOrSell(SelectedPortfolioStock, tradeStockOperation);
            }
            ((RelayCommand<object>)BuyStocksWithConditionCommand).RaiseCanExecuteChanged();
            ((RelayCommand<object>)SellStocksWithConditionCommand).RaiseCanExecuteChanged();
            ((RelayCommand<object>)BuyStockCommand).RaiseCanExecuteChanged();
            ((RelayCommand<object>)SellStockCommand).RaiseCanExecuteChanged();
        }
        private void UpdateFinancialPortfolioAfterBuyOrSell(StockViewModel stockTraded, TradeStockOperation operation)
        {
            if (operation == TradeStockOperation.BuyStock)
            {
                FinancialPortfolio.Money -= stockTraded.Price;
                if (FinancialPortfolio.StocksPortfolio.Any(x => x.Symbol == stockTraded.Symbol))
                {
                    FinancialPortfolio.StocksPortfolio.First(x => x.Symbol == stockTraded.Symbol).Quantity++;
                }
                else
                {
                    FinancialPortfolio.StocksPortfolio.Add(new StockViewModel(
                    new StockModel()
                    {
                        Symbol = stockTraded.Symbol,
                        Quantity = 1,
                        Price = stockTraded.Price
                    }));
                }
            }
            else if (operation == TradeStockOperation.SellStock)
            {
                FinancialPortfolio.Money += stockTraded.Price;
                if (Stocks.Any(x => x.Symbol == stockTraded.Symbol))
                {
                    Stocks.First(x => x.Symbol == stockTraded.Symbol).Quantity++;
                }
            }
        }
        private bool CanBuyOrSellStocks(TradeStockOperation tradeStockOperation)
        {
            if (tradeStockOperation == TradeStockOperation.BuyStock)
            {
                return SelectedStock != null && SelectedStock.Quantity > 0;
            }
            else if (tradeStockOperation == TradeStockOperation.SellStock)
            {
                return SelectedPortfolioStock != null && SelectedPortfolioStock.Quantity > 0;
            }
            return false;
        }
        private void ShowMessageOverlay(string message)
        {
            MessageOverlayText = message;
            IsMessageOverlayVisible = true;
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
                var newPrice = random.Next(InitialSettingsDict["LOWEST_STOCK_PRICE"], InitialSettingsDict["HIGHEST_STOCK_PRICE"] + 1) + random.Next(100) / 100.0f;
                stock.Price = newPrice;

                var portfolioStock = FinancialPortfolio.StocksPortfolio.FirstOrDefault(ps => ps.Symbol == stock.Symbol);
                if (portfolioStock != null)
                {
                    portfolioStock.Price = newPrice;
                }
            }
            RaisePropertyChanged(nameof(Stocks));
            RaisePropertyChanged(nameof(FinancialPortfolio));
            ((RelayCommand<object>)BuyStocksWithConditionCommand).RaiseCanExecuteChanged();
            ((RelayCommand<object>)SellStocksWithConditionCommand).RaiseCanExecuteChanged();
            ((RelayCommand<object>)BuyStockCommand).RaiseCanExecuteChanged();
            ((RelayCommand<object>)SellStockCommand).RaiseCanExecuteChanged();
            _logger.LogInfo("Stock prices updated");
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
                if (FinancialPortfolio.Money >= InitialSettingsDict["WINNING_MONEY"])
                {
                    StopTimers();
                    ShowMessageOverlay($"Congratulations! You win! you reached ${InitialSettingsDict["WINNING_MONEY"]}, clicking ok will restart the game");
                }
                else if (FinancialPortfolio.Money < InitialSettingsDict["LOSING_MONEY"])
                {
                    StopTimers();
                    ShowMessageOverlay($"Game Over! You Lose! you have less than ${InitialSettingsDict["LOSING_MONEY"]}, you can try again if you wish, clicking ok will restart the game");
                }
            }
        }
        #endregion
        #region Restart event handler
        private void Restart()
        {
            _logger.LogInfo("Restart called");
            InitializationOnRestart();
        }
        #endregion
        #region CloseMessage event handler
        private void CloseMessageOverlay()
        {
            Restart();
            IsMessageOverlayVisible = false;
        }
        #endregion
        #region BuyStocksWithCondition event handler
        private void BuyOrSellAllStocksByPriceCondition(TradeStockOperation tradeStockOperation)
        {
            Predicate<StockViewModel> priceConditionPredicate = x => x.Quantity > 0 && SelectedPriceCondition switch
            {
                PriceCondition.Above => x.Price > PriceConditionAmount,
                PriceCondition.Below => x.Price < PriceConditionAmount,
                PriceCondition.Equal => x.Price == PriceConditionAmount,
                _ => throw new ArgumentOutOfRangeException(nameof(SelectedPriceCondition), SelectedPriceCondition, null)
            };
            List<StockViewModel> stocksToTrade;
            if (tradeStockOperation == TradeStockOperation.BuyStock)
            {
                stocksToTrade = Stocks.Where(x => priceConditionPredicate(x)).ToList();
            }
            else
            {
                stocksToTrade = FinancialPortfolio.StocksPortfolio.Where(x => priceConditionPredicate(x)).ToList();
            }
            if (stocksToTrade.Any())
            {
                stocksToTrade.ForEach(x =>
                {
                    x.Quantity--;
                    UpdateFinancialPortfolioAfterBuyOrSell(x, tradeStockOperation);
                });
                ((RelayCommand<object>)BuyStocksWithConditionCommand).RaiseCanExecuteChanged();
                ((RelayCommand<object>)SellStocksWithConditionCommand).RaiseCanExecuteChanged();
                ((RelayCommand<object>)BuyStockCommand).RaiseCanExecuteChanged();
                ((RelayCommand<object>)SellStockCommand).RaiseCanExecuteChanged();
            }
        }
        private bool CanBuyOrSellStocksWithCondition(TradeStockOperation tradeStockOperation)
        {
            if (PriceConditionAmount > 0)
            {
                Predicate<StockViewModel> priceConditionPredicate = x => x.Quantity > 0 && SelectedPriceCondition switch
                {
                    PriceCondition.Above => x.Price > PriceConditionAmount,
                    PriceCondition.Below => x.Price < PriceConditionAmount,
                    PriceCondition.Equal => x.Price == PriceConditionAmount,
                    _ => throw new ArgumentOutOfRangeException(nameof(SelectedPriceCondition), SelectedPriceCondition, null)
                };
                if (tradeStockOperation == TradeStockOperation.BuyStock)
                {
                    return Stocks.Any(stock => priceConditionPredicate(stock));
                }
                else
                {
                    return FinancialPortfolio.StocksPortfolio.Any(stock => priceConditionPredicate(stock));
                }
            }
            return false;
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
                _elapsedTimeTimer.Tick -= (s, e) => ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(InitialSettingsDict["TIMER_ELAPSED_TIME_INTERVAL_IN_SECONDS"]));
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
            _stockRepository?.Clear();
        }
        #endregion
    }
}