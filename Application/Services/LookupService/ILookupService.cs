using Application.Services.LookupService.DTOs;

namespace Application.Services.LookupService
{
    public interface ILookupService
    {
        Task<List<GetLookupResponse>> GetAllServiceCategories();
    }
}
