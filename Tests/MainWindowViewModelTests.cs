using Xunit;
using StockTradingApplication.ViewModels;
using StockTradingApplication.Models;

namespace Tests
{
    public class MainViewModelTests : IDisposable
    {
        private MainViewModel _viewModel;

        public MainViewModelTests()
        {
            _viewModel = new MainViewModel();
        }

        [Fact]
        public void Initialization_ShouldFillStocksInitializeFinancialPortfolioAndInitializeCommands()
        {
            // Assert
            Assert.NotNull(_viewModel.Stocks);
            Assert.NotEmpty(_viewModel.Stocks);
            Assert.NotNull(_viewModel.FinancialPortfolio);
            Assert.Equal(1000.0f, _viewModel.FinancialPortfolio.Money);
            Assert.Empty(_viewModel.FinancialPortfolio.StocksPortfolio);
            Assert.Null(_viewModel.SelectedStock);
            Assert.Null(_viewModel.SelectedPortfolioStock);

            Assert.NotNull(_viewModel.BuyStockCommand);
            Assert.NotNull(_viewModel.SellStockCommand);
            Assert.NotNull(_viewModel.RestartCommand);
            Assert.NotNull(_viewModel.CloseMessageOverlayCommand);

            Assert.NotNull(_viewModel.ElapsedTime);
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
        public void FinancialPortfolio_PropertyChanged_ShouldShowWinningMessage()
        {
            // Arrange
            _viewModel.FinancialPortfolio.Money = 9999.99f; // Just below winning threshold

            // Act
            _viewModel.FinancialPortfolio.Money = 10000; // Trigger the condition

            // Assert
            Assert.True(_viewModel.IsMessageOverlayVisible);
            Assert.Equal("Congratulations! You win! you reached $10000, clicking ok will restart the game", _viewModel.MessageOverlayText);
        }

        [Fact]
        public void FinancialPortfolio_PropertyChanged_ShouldShowLosingMessage()
        {
            // Arrange
            _viewModel.FinancialPortfolio.Money = 0.01f; // Just above losing threshold

            // Act
            _viewModel.FinancialPortfolio.Money = 0; // Trigger the condition

            // Assert
            Assert.True(_viewModel.IsMessageOverlayVisible);
            Assert.Equal("Game Over! You Lose! you have less than $1, you can try again if you wish, clicking ok will restart the game", _viewModel.MessageOverlayText);
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
            Assert.NotEqual(initialStockPrices, pricesAfterOneMinute);
            Assert.NotEqual(initialStockPrices, pricesAfterTwoMinutes);
            Assert.NotEqual(pricesAfterOneMinute, pricesAfterTwoMinutes);

            Assert.NotNull(_viewModel.FinancialPortfolio);
            Assert.NotEmpty(_viewModel.FinancialPortfolio.StocksPortfolio);
            Assert.NotEqual(initialPortfolioPrices, portfolioPricesAfterOneMinute);
            Assert.NotEqual(initialPortfolioPrices, portfolioPricesAfterTwoMinutes);
            Assert.NotEqual(portfolioPricesAfterOneMinute, portfolioPricesAfterTwoMinutes);
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