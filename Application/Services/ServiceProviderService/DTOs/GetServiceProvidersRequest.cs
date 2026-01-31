using Application.Generic_DTOs;

namespace Application.Services.ServiceProviderService.DTOs
{
    public class GetServiceProvidersRequest : PaginationRequest
    {
        public string? SearchTerm { get; set; }
    }
}
