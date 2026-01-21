using Domain.Enums;

namespace Application.Services.OrderService.DTOs
{
    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
