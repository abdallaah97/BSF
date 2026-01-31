namespace Application.Services.ServiceProviderService.DTOs
{
    public class GetServiceProviderAccountResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ServiceCategoryId { get; set; }
        public bool IsAvailable { get; set; }
        public string? PersonalPhoto { get; set; }
        public int ServicesCount { get; set; }
        public int CompletedOrdersCount { get; set; }
    }
}
