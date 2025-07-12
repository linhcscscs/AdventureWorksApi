using AdventureWorks.Domain.DTO;
using AdventureWorks.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorksAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LowStockApiController : ControllerBase
{
    #region Props
    private readonly AdventureWorks2022Context _context;
    public LowStockApiController(AdventureWorks2022Context context)
    {
        _context = context;
    }
    #endregion
    #region API
    [HttpGet]
    public async Task<IActionResult> GetLowStockAlert(DateTime date)
    {
        try
        {
            DateTime threeMonthsAgo = date.AddMonths(-3);

            var avgSoldProducts = from sod in _context.SalesOrderDetails
                                  join soh in _context.SalesOrderHeaders on sod.SalesOrderID equals soh.SalesOrderID
                                  where soh.OrderDate >= threeMonthsAgo
                                  group sod by sod.ProductID into g
                                  select new
                                  {
                                      ProductID = g.Key,
                                      AvgSoldLastThreeMonths = (decimal)g.Sum(x => x.OrderQty) / 3
                                  };

            var alertData = await (from p in _context.Products
                                   join pi in _context.ProductInventories on p.ProductID equals pi.ProductID
                                   join avp in avgSoldProducts on p.ProductID equals avp.ProductID
                                   where pi.Quantity < 2 * avp.AvgSoldLastThreeMonths
                                   select new LowStockDto
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.Name,
                                       StockQtty = pi.Quantity,
                                       AvgSoldLastThreeMonths = Math.Round(avp.AvgSoldLastThreeMonths, 0),
                                       ExpectedShortage = Math.Round(2 * avp.AvgSoldLastThreeMonths - pi.Quantity, 0)
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
    #endregion
}
