using Xunit;
using StockTradingApplication.ViewModels;
using StockTradingApplication.Models;

namespace Tests
{
    public class MainViewModelTests : IDisposable
    {
        #region Constants
        private const float LOWEST_CHANGE_IN_MONEY = 0.01f;
        #endregion
        private MainViewModel _viewModel;

        public MainViewModelTests()
        {
            _viewModel = new MainViewModel();
        }

        [Fact]
        public void Initialization_ShouldFillStocksInitializeFinancialPortfolioAndInitializeCommands()
        {
            // Assert
            Assert.NotNull(_viewModel);
            Assert.NotNull(_viewModel.InitialSettingsDict);
            Assert.NotEmpty(_viewModel.InitialSettingsDict);
            Assert.NotNull(_viewModel.BuyStockCommand);
            Assert.NotNull(_viewModel.SellStockCommand);
            Assert.NotNull(_viewModel.RestartCommand);
            Assert.NotNull(_viewModel.CloseMessageOverlayCommand);
            Assert.NotNull(_viewModel.BuyStocksWithConditionCommand);
            Assert.NotNull(_viewModel.SellStocksWithConditionCommand);

            Assert.NotNull(_viewModel.PriceConditionOptions);
            Assert.NotEmpty(_viewModel.PriceConditionOptions);

            Assert.NotNull(_viewModel.Stocks);
            Assert.NotEmpty(_viewModel.Stocks);
            Assert.NotNull(_viewModel.FinancialPortfolio);
            Assert.Equal(1000.0f, _viewModel.FinancialPortfolio.Money);
            Assert.Empty(_viewModel.FinancialPortfolio.StocksPortfolio);
            Assert.Null(_viewModel.SelectedStock);
            Assert.Null(_viewModel.SelectedPortfolioStock);

            Assert.Equal(default(TimeSpan), _viewModel.ElapsedTime);
        }

        [Fact]
        public void BuyStockCommand_ShouldDecreaseSelectedStockQuantity()
        {
            // Arrange
            _viewModel.SelectedStock = _viewModel.Stocks[0];
            var initialQuantity = _viewModel.SelectedStock.Quantity;

            // Act
            _viewModel.BuyStockCommand.Execute(null);

            // Assert
            Assert.Equal(initialQuantity - 1, _viewModel.SelectedStock.Quantity);
        }

        [Fact]
        public void SellStockCommand_ShouldIncreaseStockQuantityInStocks()
        {
            // Arrange
            _viewModel.FinancialPortfolio.StocksPortfolio.Add(new StockViewModel(new StockModel { Symbol = "AAPL", Quantity = 1, Price = 150 }));
            var initialPortfolioQuantity = _viewModel.FinancialPortfolio.StocksPortfolio[0].Quantity;

            // Act
            _viewModel.SelectedPortfolioStock = _viewModel.FinancialPortfolio.StocksPortfolio[0]; // Select AAPL from portfolio
            _viewModel.SellStockCommand.Execute(null);

            // Assert
            Assert.Equal(initialPortfolioQuantity - 1, _viewModel.FinancialPortfolio.StocksPortfolio[0].Quantity);
        }

        [Fact]
        public void CanBuyStock_ShouldReturnFalse_WhenNoStockIsSelected()
        {
            // Arrange
            _viewModel.SelectedStock = null;

            // Act
            var canExecute = _viewModel.BuyStockCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void CanSellStock_ShouldReturnFalse_WhenNoStockIsSelected()
        {
            // Arrange
            _viewModel.SelectedPortfolioStock = null;

            // Act
            var canExecute = _viewModel.SellStockCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void FinancialPortfolio_PropertyChanged_ShouldShowWinningMessage()
        {
            // Arrange
            _viewModel.FinancialPortfolio.Money = _viewModel.InitialSettingsDict["WINNING_MONEY"] - LOWEST_CHANGE_IN_MONEY; // Just below winning threshold

            // Act
            _viewModel.FinancialPortfolio.Money += LOWEST_CHANGE_IN_MONEY; // Trigger the condition

            // Assert
            Assert.True(_viewModel.IsMessageOverlayVisible);
            Assert.Equal($"Congratulations! You win! you reached ${_viewModel.InitialSettingsDict["WINNING_MONEY"]}, clicking ok will restart the game", _viewModel.MessageOverlayText);
        }

        [Fact]
        public void FinancialPortfolio_PropertyChanged_ShouldShowLosingMessage()
        {
            // Arrange
            _viewModel.FinancialPortfolio.Money = _viewModel.InitialSettingsDict["LOSING_MONEY"]; // Just above losing threshold

            // Act
            _viewModel.FinancialPortfolio.Money -= LOWEST_CHANGE_IN_MONEY; // Trigger the condition

            // Assert
            Assert.True(_viewModel.IsMessageOverlayVisible);
            Assert.Equal($"Game Over! You Lose! you have less than ${_viewModel.InitialSettingsDict["LOSING_MONEY"]}, you can try again if you wish, clicking ok will restart the game", _viewModel.MessageOverlayText);
        }
        [Fact]
        public void StockPrices_ShouldUpdateAfterOneAndTwoMinutes()
        {
            // Arrange
            _viewModel.FinancialPortfolio.StocksPortfolio.Add(_viewModel.Stocks[0]);

            var initialStockPrices = _viewModel.Stocks.Select(s => s.Price).ToList();
            var initialPortfolioPrices = _viewModel.FinancialPortfolio.StocksPortfolio.Select(s => s.Price).ToList();

            // Act - Simulate 1 minute delay
            _viewModel.SimulateTick();
            var pricesAfterOneMinute = _viewModel.Stocks.Select(s => s.Price).ToList();
            var portfolioPricesAfterOneMinute = _viewModel.FinancialPortfolio.StocksPortfolio.Select(s => s.Price).ToList();

            // Act - Simulate another 1 minute delay
            _viewModel.SimulateTick();
            var pricesAfterTwoMinutes = _viewModel.Stocks.Select(s => s.Price).ToList();
            var portfolioPricesAfterTwoMinutes = _viewModel.FinancialPortfolio.StocksPortfolio.Select(s => s.Price).ToList();

            // Assert
            Assert.NotNull(_viewModel.Stocks);
            Assert.NotEmpty(_viewModel.Stocks);
            Assert.True(!initialStockPrices.Any(p => p < _viewModel.InitialSettingsDict["LOWEST_STOCK_PRICE"] || p > _viewModel.InitialSettingsDict["HIGHEST_STOCK_PRICE"]));
            Assert.NotEqual(initialStockPrices, pricesAfterOneMinute);
            Assert.True(!pricesAfterOneMinute.Any(p => p < _viewModel.InitialSettingsDict["LOWEST_STOCK_PRICE"] || p > _viewModel.InitialSettingsDict["HIGHEST_STOCK_PRICE"]));
            Assert.NotEqual(initialStockPrices, pricesAfterTwoMinutes);
            Assert.True(!pricesAfterTwoMinutes.Any(p => p < _viewModel.InitialSettingsDict["LOWEST_STOCK_PRICE"] || p > _viewModel.InitialSettingsDict["HIGHEST_STOCK_PRICE"]));
            Assert.NotEqual(pricesAfterOneMinute, pricesAfterTwoMinutes);

            Assert.NotNull(_viewModel.FinancialPortfolio);
            Assert.NotEmpty(_viewModel.FinancialPortfolio.StocksPortfolio);
            Assert.NotEqual(initialPortfolioPrices, portfolioPricesAfterOneMinute);
            Assert.NotEqual(initialPortfolioPrices, portfolioPricesAfterTwoMinutes);
            Assert.NotEqual(portfolioPricesAfterOneMinute, portfolioPricesAfterTwoMinutes);
        }

        [Theory]
        [InlineData(MainViewModel.PriceCondition.Above)]
        [InlineData(MainViewModel.PriceCondition.Below)]
        [InlineData(MainViewModel.PriceCondition.Equal)]
        public void BuyStockWithCondition_ShouldBuyTheStockAccordingToCondition(MainViewModel.PriceCondition condition)
        {
            // Arrange
            int priceChangeUponCondition = condition switch
            {
                MainViewModel.PriceCondition.Above => -1,
                MainViewModel.PriceCondition.Below => 1,
                MainViewModel.PriceCondition.Equal => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
            _viewModel.SelectedPriceCondition = condition;
            _viewModel.PriceConditionAmount = _viewModel.Stocks[0].Price + priceChangeUponCondition;
            var qualifiedStock = _viewModel.Stocks
                .FirstOrDefault(x =>
                    condition switch
                    {
                        MainViewModel.PriceCondition.Above => x.Price > _viewModel.PriceConditionAmount,
                        MainViewModel.PriceCondition.Below => x.Price < _viewModel.PriceConditionAmount,
                        MainViewModel.PriceCondition.Equal => x.Price == _viewModel.PriceConditionAmount,
                        _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
                    });
            int qualifiedStockInitialQuantity = qualifiedStock?.Quantity ?? 0;

            // Act
            _viewModel.BuyStocksWithConditionCommand.Execute(null);

            // Assert
            Assert.NotNull(qualifiedStock);
            Assert.True(qualifiedStockInitialQuantity > 0);
            Assert.True(_viewModel.FinancialPortfolio.StocksPortfolio.Any(x => x.Symbol == qualifiedStock.Symbol));
            Assert.True(_viewModel.FinancialPortfolio.StocksPortfolio.First(x => x.Symbol == qualifiedStock.Symbol).Quantity > 0);
            Assert.Equal(qualifiedStockInitialQuantity - 1, qualifiedStock.Quantity);
            Assert.True(_viewModel.FinancialPortfolio.Money <= _viewModel.InitialSettingsDict["STARTING_MONEY"] - qualifiedStock.Price);
        }

        [Theory]
        [InlineData(MainViewModel.PriceCondition.Above)]
        [InlineData(MainViewModel.PriceCondition.Below)]
        [InlineData(MainViewModel.PriceCondition.Equal)]
        public void SellStockWithCondition_ShouldSellTheStockAccordingToCondition(MainViewModel.PriceCondition condition)
        {
            // Arrange
            _viewModel.FinancialPortfolio.StocksPortfolio.Add(new StockViewModel(
                    new StockModel()
                    {
                        Symbol = _viewModel.Stocks[0].Symbol,
                        Quantity = 1,
                        Price = _viewModel.Stocks[0].Price
                    }));
            int priceChangeUponCondition = condition switch
            {
                MainViewModel.PriceCondition.Above => -1,
                MainViewModel.PriceCondition.Below => 1,
                MainViewModel.PriceCondition.Equal => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
            };
            _viewModel.SelectedPriceCondition = condition;
            _viewModel.PriceConditionAmount = _viewModel.FinancialPortfolio.StocksPortfolio[0].Price + priceChangeUponCondition;
            var qualifiedFromPortfolioStock = _viewModel.FinancialPortfolio.StocksPortfolio
                .FirstOrDefault(x =>
                    condition switch
                    {
                        MainViewModel.PriceCondition.Above => x.Price > _viewModel.PriceConditionAmount,
                        MainViewModel.PriceCondition.Below => x.Price < _viewModel.PriceConditionAmount,
                        MainViewModel.PriceCondition.Equal => x.Price == _viewModel.PriceConditionAmount,
                        _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
                    });
            int qualifiedStockInitialQuantity = qualifiedFromPortfolioStock?.Quantity ?? 0;

            // Act
            _viewModel.SellStocksWithConditionCommand.Execute(null);

            // Assert
            Assert.NotNull(qualifiedFromPortfolioStock);
            Assert.True(qualifiedStockInitialQuantity > 0);
            Assert.True(_viewModel.Stocks.Any(x => x.Symbol == qualifiedFromPortfolioStock.Symbol));
            Assert.True(_viewModel.Stocks.First(x => x.Symbol == qualifiedFromPortfolioStock.Symbol).Quantity > 0);
            Assert.Equal(qualifiedStockInitialQuantity - 1, qualifiedFromPortfolioStock.Quantity);
            Assert.True(_viewModel.FinancialPortfolio.Money >= qualifiedFromPortfolioStock.Price);
        }

        public void Dispose()
        {
            if (_viewModel != null)
            {
                _viewModel.Dispose();
                _viewModel = null;
            }
        }
    }
}