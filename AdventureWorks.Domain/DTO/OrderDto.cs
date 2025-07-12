using AdventureWorks.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Domain.DTO
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public decimal TotalDue { get; set; }
        public OrderStatus Status { get; set; }
        public List<SalesOrderDetailDto> OrderDetails { get; set; } = new List<SalesOrderDetailDto>();
    }

    public class SalesOrderDetailDto
    {
        public int ProductID { get; set; }
        public short OrderQty { get; set; }
    }
}
