using Application.Generic_DTOs;
using Application.Services.ClientUserService.DTOs;
using Application.Services.ServiceProviderService.DTOs;

namespace Application.Services.ServiceProviderService
{
    public interface IServiceProviderService
    {
        Task ServiceProviderRegistration(ServiceProviderRegistrationRequest request);
        Task<GetServiceProviderAccountResponse> GetServiceProviderAccount();
        Task UpdateServiceProviderAccount(UpdateServiceProviderRequest request);
        Task<PaginationResponse<GetServiceProviderAccountResponse>> GetAllServiceProviders(GetServiceProvidersRequest request);
    }
}
