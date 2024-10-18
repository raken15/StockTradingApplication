using Xunit;
using StockTradingApplication.ViewModels;
using StockTradingApplication.Models;
using System.Collections.ObjectModel;

namespace Tests
{
    public class MainViewModelTests: IDisposable
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
            Assert.NotNull(_viewModel.CloseMessageCommand);

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
            Assert.True(_viewModel.IsMessageVisible);
            Assert.Equal("Congratulations! You win! you reached $10000, clicking ok will restart the game", _viewModel.Message);
        }

        [Fact]
        public void FinancialPortfolio_PropertyChanged_ShouldShowLosingMessage()
        {
            // Arrange
            _viewModel.FinancialPortfolio.Money = 0.01f; // Just above losing threshold

            // Act
            _viewModel.FinancialPortfolio.Money = 0; // Trigger the condition

            // Assert
            Assert.True(_viewModel.IsMessageVisible);
            Assert.Equal("Game Over! You Lose! you have less than $1, you can try again if you wish, clicking ok will restart the game", _viewModel.Message);
        }

        public void Dispose()
        {
            // Cleanup logic here, if necessary
            // This will be called after each test method
            if (_viewModel != null)
            {
                _viewModel.FinancialPortfolio.StocksPortfolio.Clear();
                _viewModel.Stocks.Clear();
                _viewModel.SelectedStock = null;
                _viewModel.SelectedPortfolioStock = null;
                _viewModel = null; // or other cleanup actions
            }
        }
    }
}
