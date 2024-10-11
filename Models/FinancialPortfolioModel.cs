using System;

namespace StockTradingApplication.Models;

public class FinancialPortfolioModel
{
    public List<StockModel> Stocks { get; set; }
    public decimal Money { get; set; }

    public FinancialPortfolioModel()
    {
        Stocks = new List<StockModel>();
    }
}
