namespace AdventureWorks.Domain.DTO
{
    public class LowStockDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int StockQtty { get; set; }
        public decimal AvgSoldLast3Months { get; set; }
        public decimal ExpectedShortage { get; set; }
    }
}
