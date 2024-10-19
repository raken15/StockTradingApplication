namespace StockTradingApplication.Models;

/// <summary>
/// Represents a financial portfolio consisting of multiple stocks and a monetary balance.
/// Provides functionality for initializing the portfolio with an empty list of stocks.
/// </summary>
public class FinancialPortfolioModel
{
    public List<StockModel> Stocks { get; set; }
    public float Money { get; set; }

    public FinancialPortfolioModel()
    {
        Stocks = new List<StockModel>();
    }
}
