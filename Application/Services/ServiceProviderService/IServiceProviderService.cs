using Application.Services.ServiceProviderService.DTOs;

namespace Application.Services.ServiceProviderService
{
    public interface IServiceProviderService
    {
        Task ServiceProviderRegistration(ServiceProviderRegistrationRequest request);
        Task<GetServiceProviderAccountResponse> GetServiceProviderAccount();
    }
}
