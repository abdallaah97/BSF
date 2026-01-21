using Application.Services.OrderService.DTOs;

namespace Application.Services.OrderService
{
    public interface IOrderService
    {
        Task RequestOrder(SaveOrderRequest request);
        Task<List<GetOrderResponse>> GetClientUserOrders();
        Task UpdateOrderStatus(UpdateOrderStatusRequest request);
        Task DeleteOrder(int orderId);
    }
}
