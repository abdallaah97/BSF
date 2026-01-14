using Domain.Enums;

namespace Application.Services.LookupService.DTOs
{
    public class GetLookupResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ServiceCategoryeEnum Code { get; set; }
    }
}
