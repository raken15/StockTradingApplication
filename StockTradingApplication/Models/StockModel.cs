
namespace StockTradingApplication.Models
{
    /// <summary>
    /// Represents a stock with its symbol, quantity, and price.
    /// </summary>
    public class StockModel
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}