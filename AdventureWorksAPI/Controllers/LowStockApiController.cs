using AdventureWorks.Domain.DTO;
using AdventureWorks.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LowStockApiController : ControllerBase
{
    private readonly AdventureWorks2022Context _context;
    public LowStockApiController(AdventureWorks2022Context context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetLowStockAlert()
    {
        try
        {
            DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);
            // AVG Sold Product
            var avgSoldProducts = from sod in _context.SalesOrderDetails
                                  join soh in _context.SalesOrderHeaders on sod.SalesOrderID equals soh.SalesOrderID
                                  where soh.OrderDate >= threeMonthsAgo
                                  group sod by sod.ProductID into g
                                  select new
                                  {
                                      ProductID = g.Key,
                                      AvgSoldLast3Months = g.Sum(x => x.OrderQty) / 3
                                  };

            // Alert Data
            var alertData = await (from p in _context.Products
                                   join pi in _context.ProductInventories on p.ProductID equals pi.ProductID
                                   join avp in avgSoldProducts on p.ProductID equals avp.ProductID
                                   let stock = pi.Quantity
                                   let avgSold = avp.AvgSoldLast3Months
                                   where stock < 2 * avgSold
                                   select new LowStockDto
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.Name,
                                       StockQtty = stock,
                                       AvgSoldLast3Months = Math.Round((decimal)avgSold, 2),
                                       ExpectedShortage = Math.Round(2 * (decimal)avgSold - stock, 2)
                                   }).ToListAsync();

            if (alertData == null || !alertData.Any())
                return NoContent();

            return Ok(alertData);
        }
        catch (Exception ex)
        {
            return StatusCode(500);
        }
    }
}
