using Application.Generic_DTOs;

namespace Application.Services.Service.DTOs
{
    public class GetServicesRequest : PaginationRequest
    {
        public string? SearchTerm { get; set; }
    }
}
