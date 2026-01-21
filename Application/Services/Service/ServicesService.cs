using Application.Generic_DTOs;
using Application.Repositories;
using Application.Services.CurrentUserService;
using Application.Services.Service.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Service
{
    public class ServicesService : IServicesService
    {
        private readonly IGenericRepository<Domain.Entittes.Service> _serviceRepo;
        private readonly ICurrentUserService _currentUserService;
        public ServicesService(IGenericRepository<Domain.Entittes.Service> serviceRepo, ICurrentUserService currentUserService)
        {
            _serviceRepo = serviceRepo;
            _currentUserService = currentUserService;
        }

        public async Task CreateService(SaveServiceRequest request)
        {
            var service = new Domain.Entittes.Service
            {
                Name = request.Name,
                ServiceProviderId = _currentUserService.ServiceProviderId.Value,
                Duration = request.Duration,
                Price = request.Price,
                Description = request.Description
            };

            await _serviceRepo.InsertAsync(service);
            await _serviceRepo.SaveChangesAsync();
        }

        public async Task DeleteService(int id)
        {
            var service = await _serviceRepo.GetByIdAsync(id);

            if (service == null)
            {
                throw new Exception("Service not exist");
            }

            _serviceRepo.Delete(service);
            await _serviceRepo.SaveChangesAsync();
        }

        public async Task<PaginationResponse<GetServicesResponse>> GetMyServices(PaginationRequest request)
        {
            var serviceProviderId = _currentUserService.ServiceProviderId.Value;

            var qurey = _serviceRepo.GetAll().OrderByDescending(x => x.Id)
                .Where(x => x.ServiceProviderId == serviceProviderId)
                .Skip(request.PageSize * request.PageIndex)
                .Take(request.PageSize);

            var count = await qurey.CountAsync();

            var result = await qurey.Select(x => new GetServicesResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Duration = x.Duration,
                Price = x.Price
            }).ToListAsync();

            return new PaginationResponse<GetServicesResponse>
            {
                Items = result,
                Count = count
            };
        }

        public async Task UpdateService(int id, SaveServiceRequest request)
        {
            var service = await _serviceRepo.GetByIdAsync(id);

            service.Name = request.Name;
            service.Price = request.Price;
            service.Description = request.Description;
            service.Duration = request.Duration;

            _serviceRepo.Update(service);
            await _serviceRepo.SaveChangesAsync();
        }


    }
}
