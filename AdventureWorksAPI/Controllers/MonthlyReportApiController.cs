using AdventureWorks.Domain.DTO;
using AdventureWorks.Infrastructure.CacheProvider.BaseCache.Interface;
using AdventureWorks.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AdventureWorks.Domain.DTO.MonthlyReportDto;

namespace AdventureWorksAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MonthlyReportApiController : ControllerBase
{
    private readonly AdventureWorks2022Context _context;
    private readonly ICacheProvider _cache;
    private const int NUMBER_OF_TOP_SALE_PRODUCTS = 5;
    public MonthlyReportApiController(AdventureWorks2022Context context, ICacheProvider cache)
    {
        _context = context;
        _cache = cache;
    }
    [HttpGet]
    public async Task<IActionResult> GetMonthlyReport()
    {
        var today = DateTime.Today;
        var startOfThisMonth = new DateTime(today.Year, today.Month, 1);
        var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);
        var startOfLastMonth = startOfThisMonth.AddMonths(-1);
        var endOfLastMonth = startOfThisMonth.AddDays(-1);
        try
        {
            var data = _cache.GetOrSet(
                getDataSource: async () =>
                {
                    var thisMonthData = GetMonthlyData(startOfThisMonth, endOfThisMonth);
                    var lastMonthData = GetMonthlyData(startOfLastMonth, endOfLastMonth);
                    var topSaleProducts = (from soh in _context.SalesOrderHeaders
                                           join sod in _context.SalesOrderDetails on soh.SalesOrderID equals sod.SalesOrderID
                                           join p in _context.Products on sod.ProductID equals p.ProductID
                                           where soh.OrderDate >= startOfThisMonth && soh.OrderDate <= endOfThisMonth
                                           group sod by new { p.ProductID, p.Name } into g
                                           select new
                                           {
                                               ProductID = g.Key.ProductID,
                                               ProductName = g.Key.Name,
                                               Qty = g.Sum(x => x.OrderQty)
                                           })
                             .OrderByDescending(x => x.Qty)
                             .Take(NUMBER_OF_TOP_SALE_PRODUCTS)
                             .Select(x => new ProductReport
                             {
                                 ProductID = x.ProductID,
                                 ProductName = x.ProductName
                             })
                             .ToListAsync();
                    await Task.WhenAll(thisMonthData, lastMonthData, topSaleProducts);
                    return new MonthlyReportDto
                    {
                        Month = today.Month,
                        Year = today.Year,
                        TotalOrders = thisMonthData.Result.totalOrder,
                        TotalAmount = thisMonthData.Result.totalAmount,
                        TopSaleProduct = topSaleProducts.Result,
                        GrowthRate = new GrowthRateReport
                        {
                            GrowthRateOrders = lastMonthData.Result.totalOrder == 0 ? null :
                                Math.Round((double)(thisMonthData.Result.totalOrder - lastMonthData.Result.totalOrder) / lastMonthData.Result.totalOrder * 100, 2),
                            GrowthRateAmount = lastMonthData.Result.totalAmount == 0 ? null :
                                Math.Round((double)((thisMonthData.Result.totalAmount - lastMonthData.Result.totalAmount) / lastMonthData.Result.totalAmount * 100), 2)
                        }
                    };
                },
                key: $"MonthlyReport-{today.Year}-{today.Month}"
                );

            if (data == null)
                return NoContent();

            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }

    private async Task<(int totalOrder, decimal totalAmount)> GetMonthlyData(DateTime startDate, DateTime endDate)
    {
        var thisMonthData = _context.SalesOrderHeaders
                                    .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate);

        var countTask = thisMonthData.CountAsync();
        var sumTask = thisMonthData.SumAsync(x => x.TotalDue);

        await Task.WhenAll(countTask, sumTask);

        return (countTask.Result, sumTask.Result);
    }
}
