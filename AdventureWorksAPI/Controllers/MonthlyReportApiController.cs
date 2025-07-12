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
    public async Task<IActionResult> GetMonthlyReport(int fromMonth, int fromYear, int toMonth, int toYear)
    {
        try
        {
            var startMonth = new DateTime(fromYear, fromMonth, 1);
            var endMonth = new DateTime(toYear, toMonth, 1);

            var data = await _cache.GetOrSet(
                async () =>
                {
                    var reports = new List<MonthlyReportDto>();

                    var cursor = startMonth;
                    while (cursor <= endMonth)
                    {
                        var startOfThisMonth = new DateTime(cursor.Year, cursor.Month, 1);
                        var endOfThisMonth = startOfThisMonth.AddMonths(1).AddDays(-1);
                        var startOfLastMonth = startOfThisMonth.AddMonths(-1);
                        var endOfLastMonth = startOfThisMonth.AddDays(-1);

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

                        reports.Add(new MonthlyReportDto
                        {
                            Month = cursor.Month,
                            Year = cursor.Year,
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
                        });

                        cursor = cursor.AddMonths(1);
                    }

                    return reports;
                },
                key: $"MonthlyReport:{fromYear}-{fromMonth}_to_{toYear}-{toMonth}"
            );

            return data == null || !data.Any() ? NoContent() : Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    private (int totalOrder, decimal totalAmount) GetMonthlyData(DateTime startDate, DateTime endDate)
    {
        return _cache.GetOrSet(getDataSource: () =>
        {
            var thisMonthData = _context.SalesOrderHeaders
                            .Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate);

            var countTask = thisMonthData.Count();
            var sumTask = thisMonthData.Sum(x => x.TotalDue);
            return (countTask, sumTask);
        },
        key: $"MonthlyReport_GetMonthlyData_{startDate.ToString()}_{endDate.ToString()}");
    }
}
