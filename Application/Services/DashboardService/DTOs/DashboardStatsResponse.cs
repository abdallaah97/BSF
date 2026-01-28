namespace Application.Services.DashboardService.DTOs
{
    public class DashboardStatsResponse
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int TotalServices { get; set; }
        public int TotalServiceProviders { get; set; }
        public int TotalClientUsers { get; set; }
        public List<RecentOrderDTO> RecentOrders { get; set; }
    }

    public class RecentOrderDTO
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
