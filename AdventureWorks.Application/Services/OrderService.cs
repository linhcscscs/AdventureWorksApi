using AdventureWorks.Application.Interface;
using AdventureWorks.Domain.DTO;
using AdventureWorks.Domain.Value;
using AdventureWorks.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Application.Services
{
    public class OrderService : IOrderService
    {
        #region Props
        private readonly AdventureWorks2022Context _context;
        public OrderService(AdventureWorks2022Context context)
        {
            _context = context;
        }
        #endregion
        #region Methods
        public async Task<int> CreateOrderAsync(OrderDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new SalesOrderHeader
                {
                    CustomerID = dto.CustomerID,
                    OrderDate = DateTime.Now,
                    Status = (byte)OrderStatus.InProcess,
                    TotalDue = dto.TotalDue
                };

                _context.SalesOrderHeaders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var detailDto in dto.OrderDetails)
                {
                    var detail = new SalesOrderDetail
                    {
                        SalesOrderID = order.SalesOrderID,
                        ProductID = detailDto.ProductID,
                        OrderQty = detailDto.OrderQty,
                    };

                    _context.SalesOrderDetails.Add(detail);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return order.SalesOrderID;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SalesOrderHeader>> GetOrdersByCustomerAsync(int customerID)
        {
            return await _context.SalesOrderHeaders
                .Where(x => x.CustomerID == customerID)
                .Include(x => x.SalesOrderDetails)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderID, OrderStatus status)
        {
            var order = await _context.SalesOrderHeaders.FindAsync(orderID);
            if (order == null)
            {
                return false;
            }

            order.Status = (byte)status;
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion
    }
}
