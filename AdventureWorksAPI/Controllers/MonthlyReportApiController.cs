using AdventureWorks.Domain.DTO;
using AdventureWorks.Infrastructure.CacheProvider.BaseCache.Interface;
using AdventureWorks.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AdventureWorks.Domain.DTO.MonthlyReportDto;

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
    public async Task<IActionResult> GetMonthlyReport(int month, int year)
    {
        var startOfThisMonth = new DateTime(year, month, 1);
        var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);
        var startOfLastMonth = startOfThisMonth.AddMonths(-1);
        var endOfLastMonth = startOfThisMonth.AddDays(-1);

        try
        {
            var data = await _cache.GetOrSet(
                async () =>
                {
                    var thisMonthData = GetMonthlyData(startOfThisMonth, endOfThisMonth);
                    var lastMonthData = GetMonthlyData(startOfLastMonth, endOfLastMonth);

                    var topSaleProducts = await (from soh in _context.SalesOrderHeaders
                                               join sod in _context.SalesOrderDetails on soh.SalesOrderID equals sod.SalesOrderID
                                               join p in _context.Products on sod.ProductID equals p.ProductID
                                               where soh.OrderDate >= startOfThisMonth && soh.OrderDate <= endOfThisMonth
                                               group sod by new { p.ProductID, p.Name } into g
                                               orderby g.Sum(x => x.OrderQty) descending
                                               select new ProductReport
                                               {
                                                   ProductID = g.Key.ProductID,
                                                   ProductName = g.Key.Name
                                               })
                                             .Take(NUMBER_OF_TOP_SALE_PRODUCTS)
                                             .ToListAsync();

                    return new MonthlyReportDto
                    {
                        Month = month,
                        Year = year,
                        TotalOrders = thisMonthData.totalOrder,
                        TotalAmount = thisMonthData.totalAmount,
                        TopSaleProduct = topSaleProducts,
                        GrowthRate = new GrowthRateReport
                        {
                            GrowthRateOrders = lastMonthData.totalOrder == 0 ? null :
                                Math.Round((double)(thisMonthData.totalOrder - lastMonthData.totalOrder) / lastMonthData.totalOrder * 100, 2),
                            GrowthRateAmount = lastMonthData.totalAmount == 0 ? null :
                                Math.Round((double)((thisMonthData.totalAmount - lastMonthData.totalAmount) / lastMonthData.totalAmount * 100), 2)
                        }
                    };
                },
                key: $"MonthlyReport:{year}:{month}"
            );

            return data == null ? NoContent() : Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    private (int totalOrder, decimal totalAmount) GetMonthlyData(DateTime startDate, DateTime endDate)
    {
        var thisMonthData = _context.SalesOrderHeaders
                                    .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate);

        var countTask = thisMonthData.Count();
        var sumTask = thisMonthData.Sum(x => x.TotalDue);
        return (countTask, sumTask);
    }
}
