using Application.Repositories;
using Application.Services.CurrentUserService;
using Application.Services.OrderService.DTOs;
using Domain.Entittes;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Domain.Entittes.Service> _serviceRepo;
        private readonly ICurrentUserService _currentUserService;
        public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<Domain.Entittes.Service> serviceRepo, ICurrentUserService currentUserService)
        {
            _orderRepo = orderRepo;
            _serviceRepo = serviceRepo;
            _currentUserService = currentUserService;
        }

        public async Task RequestOrder(SaveOrderRequest request)
        {
            var service = await _serviceRepo.GetAll().Include(x => x.ServiceProvider).FirstOrDefaultAsync(x => x.Id == request.ServiceId);
            if (!service.ServiceProvider.IsAvailable)
            {
                throw new Exception("Servie Provider not available");
            }

            if (request.FromTime.TimeOfDay > request.ToTime.TimeOfDay)
            {
                throw new Exception("To time should be greter than from time");
            }

            var isAnyOrderExistinSameTime = await _orderRepo.GetAll().AnyAsync
                (
                    x => x.ServiceProviderId == service.ServiceProviderId &&
                    x.FromTime.TimeOfDay == request.FromTime.TimeOfDay &&
                    x.ToTime.TimeOfDay == request.ToTime.TimeOfDay
                );

            if (isAnyOrderExistinSameTime)
            {
                throw new Exception("Servie Provider not available");
            }

            var order = new Order
            {
                ServiceId = request.ServiceId,
                ServiceProviderId = service.ServiceProviderId,
                ClientUserId = _currentUserService.ClientUserId.Value,
                FromTime = request.FromTime,
                ToTime = request.ToTime,
                Note = request.Note,
                Status = OrderStatus.Pending,
                CreatedTime = DateTime.UtcNow,
            };

            await _orderRepo.InsertAsync(order);
            await _orderRepo.SaveChangesAsync();
        }

        public async Task<List<GetOrderResponse>> GetClientUserOrders()
        {
            var query = await _orderRepo.GetAll()
                .OrderByDescending(x => x.CreatedTime)
                .Where(x => x.ClientUserId == _currentUserService.ClientUserId.Value)
                .Include(x => x.Service)
                .ThenInclude(x => x.ServiceProvider)
                .ThenInclude(x => x.User)
                .Select(x => new GetOrderResponse
                {
                    Id = x.Id,
                    ServiceName = x.Service.Name,
                    ServiceProviderName = x.Service.ServiceProvider.User.Name,
                    FromTime = x.FromTime,
                    ToTime = x.ToTime,
                    Note = x.Note,
                    Status = x.Status.ToString(),
                    CreatedTime = x.CreatedTime
                }).ToListAsync();

            return query;
        }

        public async Task UpdateOrderStatus(UpdateOrderStatusRequest request)
        {
            var order = await _orderRepo.GetByIdAsync(request.OrderId);
            order.Status = request.Status;

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
        }

        public async Task DeleteOrder(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);

            if (order.Status != OrderStatus.Pending)
            {
                throw new Exception("Can't delete order not in progress");
            }

            _orderRepo.Delete(order);
            await _orderRepo.SaveChangesAsync();
        }
    }
}
