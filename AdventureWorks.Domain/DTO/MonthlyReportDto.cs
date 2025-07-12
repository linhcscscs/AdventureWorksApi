using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Domain.DTO
{
    public class MonthlyReportDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProductReport> TopSaleProduct { get; set; }
        public GrowthRateReport GrowthRate { get; set; }
        public class ProductReport
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
        }
        public class GrowthRateReport
        {
            public double? GrowthRateOrders { get; set; }
            public double? GrowthRateAmount { get; set; }
        }
    }
}
