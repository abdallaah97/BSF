using Application.Generic_DTOs;
using Application.Repositories;
using Application.Services.ClientUserService.DTOs;
using Application.Services.CurrentUserService;
using Application.Services.NotificationService;
using Application.Services.NotificationService.DTOs;
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
        private readonly INotificationService _notificationService;
        public OrderService(IGenericRepository<Order> orderRepo, IGenericRepository<Domain.Entittes.Service> serviceRepo, ICurrentUserService currentUserService, INotificationService notificationService)
        {
            _orderRepo = orderRepo;
            _serviceRepo = serviceRepo;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task RequestOrder(SaveOrderRequest request)
        {
            var service = await _serviceRepo.GetAll()
                .Include(x => x.ServiceProvider)
                .FirstOrDefaultAsync(x => x.Id == request.ServiceId);

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

            //Send notification to service provider
            await _notificationService.SendNotification(new CreateNotificationRequest
            {
                UserId = service.ServiceProvider.UserId,
                Title = "New Order Request",
                Message = $"You have a new order request for service {service.Name}.",
                Data = new Dictionary<string, string>
                {
                    { "{ServiceName}", service.Name },
                }
            });
        }

        public async Task<PaginationResponse<GetOrderResponse>> GetClientUserOrders(GetClientUserOrderRequest request)
        {
            var query = _orderRepo.GetAll()
                .OrderByDescending(x => x.CreatedTime)
                .Include(x => x.Service)
                .ThenInclude(x => x.ServiceProvider)
                .ThenInclude(x => x.User)
                .Where(x => x.ClientUserId == _currentUserService.ClientUserId.Value);

            if (!string.IsNullOrEmpty(request.ServiceName))
            {
                request.ServiceName = request.ServiceName.Trim().ToLower();
                query = query.Where(x => x.Service.Name.Trim().ToLower().Contains(request.ServiceName));
            }

            var count = await query.CountAsync();

            var result = await query
                .Skip(request.PageSize * request.PageIndex)
                .Take(request.PageSize)
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

            return new PaginationResponse<GetOrderResponse>
            {
                Items = result,
                Count = count
            };
        }

        public async Task<PaginationResponse<GetOrderResponse>> GetServiceProviderOrders(GetClientUserOrderRequest request)
        {
            var query = _orderRepo.GetAll()
                .OrderByDescending(x => x.CreatedTime)
                .Include(x => x.Service)
                .ThenInclude(x => x.ServiceProvider)
                .ThenInclude(x => x.User)
                .Where(x => x.ServiceProviderId == _currentUserService.ServiceProviderId.Value);

            if (!string.IsNullOrEmpty(request.ServiceName))
            {
                request.ServiceName = request.ServiceName.Trim().ToLower();
                query = query.Where(x => x.Service.Name.Trim().ToLower().Contains(request.ServiceName));
            }

            var count = await query.CountAsync();

            var result = await query
                .Skip(request.PageSize * request.PageIndex)
                .Take(request.PageSize)
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

            return new PaginationResponse<GetOrderResponse>
            {
                Items = result,
                Count = count
            };
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
