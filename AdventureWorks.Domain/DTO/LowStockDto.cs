namespace AdventureWorks.Domain.DTO
{
    public class LowStockDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int StockQtty { get; set; }
        public decimal AvgSoldLastThreeMonths { get; set; }
        public decimal ExpectedShortage { get; set; }
    }
}
