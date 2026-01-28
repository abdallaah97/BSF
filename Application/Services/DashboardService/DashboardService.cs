using Application.Repositories;
using Application.Services.DashboardService.DTOs;
using Domain.Entittes;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Domain.Entittes.Service> _serviceRepo;
        private readonly IGenericRepository<ServiceProvider> _providerRepo;
        private readonly IGenericRepository<ClientUser> _clientRepo;

        public DashboardService(
            IGenericRepository<Order> orderRepo,
            IGenericRepository<Domain.Entittes.Service> serviceRepo,
            IGenericRepository<ServiceProvider> providerRepo,
            IGenericRepository<ClientUser> clientRepo)
        {
            _orderRepo = orderRepo;
            _serviceRepo = serviceRepo;
            _providerRepo = providerRepo;
            _clientRepo = clientRepo;
        }

        public async Task<DashboardStatsResponse> GetDashboardStats()
        {
            var stats = new DashboardStatsResponse
            {
                TotalOrders = await _orderRepo.GetAll().CountAsync(),
                PendingOrders = await _orderRepo.GetAll().CountAsync(o => o.Status == OrderStatus.Pending),
                CompletedOrders = await _orderRepo.GetAll().CountAsync(o => o.Status == OrderStatus.Completed),
                TotalServices = await _serviceRepo.GetAll().CountAsync(),
                TotalServiceProviders = await _providerRepo.GetAll().CountAsync(),
                TotalClientUsers = await _clientRepo.GetAll().CountAsync(),
                RecentOrders = await _orderRepo.GetAll()
                    .OrderByDescending(o => o.CreatedTime)
                    .Take(5)
                    .Select(o => new RecentOrderDTO
                    {
                        Id = o.Id,
                        ClientName = o.ClientUser.User.Name,
                        ServiceName = o.Service.Name,
                        Status = o.Status.ToString(),
                        CreatedTime = o.CreatedTime
                    }).ToListAsync()
            };

            return stats;
        }
    }
}
