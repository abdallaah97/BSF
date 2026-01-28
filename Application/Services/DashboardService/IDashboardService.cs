using Application.Services.DashboardService.DTOs;

namespace Application.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardStatsResponse> GetDashboardStats();
    }
}
