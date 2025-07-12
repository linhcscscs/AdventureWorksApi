using AdventureWorks.Application.Interface;
using AdventureWorks.Domain.DTO;
using AdventureWorks.Domain.Value;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrderApiController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderApiController(IOrderService service)
    {
        _service = service;
    }

    #region API

    // POST api/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderDto dto)
    {
        try
        {
            var orderId = await _service.CreateOrderAsync(dto);
            return Ok(new { OrderID = orderId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    // GET api/orders/customer/5
    [HttpGet("api/[controller]/[action]/{customerId}")]
    public async Task<IActionResult> GetOrdersByCustomer(int customerId)
    {
        try
        {
            var orders = await _service.GetOrdersByCustomerAsync(customerId);
            if (orders == null || !orders.Any())
                return NoContent();

            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT api/orders/10/status
    [HttpPut("api/[controller]/[action]/{orderId}")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatus status)
    {
        try
        {
            var updated = await _service.UpdateOrderStatusAsync(orderId, status);
            if (!updated)
                return NotFound();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion
}
