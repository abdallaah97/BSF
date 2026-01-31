namespace Application.Services.Service.DTOs
{
    public class GetServicesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? ServiceProviderName { get; set; }
    }
}
