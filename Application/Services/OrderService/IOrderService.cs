using Application.Generic_DTOs;
using Application.Services.ClientUserService.DTOs;
using Application.Services.OrderService.DTOs;

namespace Application.Services.OrderService
{
    public interface IOrderService
    {
        Task RequestOrder(SaveOrderRequest request);
        Task<PaginationResponse<GetOrderResponse>> GetClientUserOrders(GetClientUserOrderRequest request);
        Task<PaginationResponse<GetOrderResponse>> GetServiceProviderOrders(GetClientUserOrderRequest request);
        Task<PaginationResponse<GetOrderResponse>> GetAllOrders(GetClientUserOrderRequest request);
        Task UpdateOrderStatus(UpdateOrderStatusRequest request);
        Task DeleteOrder(int orderId);
    }
}
