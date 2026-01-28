using Application.Services.ClientUserService.DTOs;
using Application.Services.OrderService;
using Application.Services.OrderService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = $"User")]
        [HttpPost("RequestOrder")]
        public async Task<IActionResult> RequestOrder([FromBody] SaveOrderRequest request)
        {
            await _orderService.RequestOrder(request);
            return Ok();
        }

        [Authorize(Roles = $"User")]
        [HttpPost("GetClientUserOrders")]
        public async Task<IActionResult> GetClientUserOrders([FromBody] GetClientUserOrderRequest request)
        {
            var response = await _orderService.GetClientUserOrders(request);
            return Ok(response);
        }

        [Authorize(Roles = $"ServiceProvider")]
        [HttpPost("GetServiceProviderOrders")]
        public async Task<IActionResult> GetServiceProviderOrders([FromBody] GetClientUserOrderRequest request)
        {
            var response = await _orderService.GetServiceProviderOrders(request);
            return Ok(response);
        }

        [Authorize(Roles = $"ServiceProvider")]
        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody]UpdateOrderStatusRequest request)
        {
            await _orderService.UpdateOrderStatus(request);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders([FromBody] GetClientUserOrderRequest request)
        {
            var response = await _orderService.GetAllOrders(request);
            return Ok(response);
        }

        [Authorize(Roles = $"User")]
        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            await _orderService.DeleteOrder(orderId);
            return Ok();
        }
    }
}
