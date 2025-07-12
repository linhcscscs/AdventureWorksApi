using AdventureWorks.Domain.DTO;
using AdventureWorks.Domain.Value;
using AdventureWorks.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Application.Interface
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(OrderDto dto);
        Task<List<SalesOrderHeader>> GetOrdersByCustomerAsync(int customerID);
        Task<bool> UpdateOrderStatusAsync(int orderID, OrderStatus status);
    }
}
