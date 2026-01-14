using Application.Repositories;
using Application.Services.CurrentUserService;
using Application.Services.ServiceProviderService.DTOs;
using Domain.Entittes;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.ServiceProviderService
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IGenericRepository<ServiceProvider> _serviceProviderRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly ICurrentUserService _currentUserService;
        public ServiceProviderService(IGenericRepository<ServiceProvider> serviceProviderRepo, IGenericRepository<User> userRepo, IGenericRepository<Role> roleRepo, ICurrentUserService currentUserService)
        {
            _serviceProviderRepo = serviceProviderRepo;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _currentUserService = currentUserService;
        }

        public async Task<GetServiceProviderAccountResponse> GetServiceProviderAccount()
        {
            var userId = _currentUserService.UserId;

            var serviceProvider = await _serviceProviderRepo.GetAll()
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);

            if (serviceProvider == null)
            {
                throw new Exception("Service provider not found");
            }

            var response = new GetServiceProviderAccountResponse
            {
                Id = serviceProvider.Id,
                Name = serviceProvider.User.Name,
                Email = serviceProvider.User.Email,
                PhoneNumber = serviceProvider.User.PhonNumber,
                ServiceCategoryId = serviceProvider.ServiceCategoryId
            };

            return response;
        }

        public async Task ServiceProviderRegistration(ServiceProviderRegistrationRequest request)
        {
            await RegistrationValidation(request);

            var serviceProviderRole = await _roleRepo.GetAll().FirstOrDefaultAsync(x => x.Code == SytemRole.ServiceProvider);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PhonNumber = request.PhonNumber,
                RoleId = serviceProviderRole.Id
            };

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, request.Password);

            await _userRepo.InsertAsync(user);
            await _userRepo.SaveChangesAsync();

            var serviceProvider = new ServiceProvider
            {
                UserId = user.Id,
                ServiceCategoryId = request.ServiceCategoryId
            };

            await _serviceProviderRepo.InsertAsync(serviceProvider);
            await _serviceProviderRepo.SaveChangesAsync();
        }

        private async Task RegistrationValidation(ServiceProviderRegistrationRequest request)
        {
            var isEmailExist = await _userRepo.GetAll().AnyAsync(x => x.Email == request.Email);
            if (isEmailExist)
            {
                throw new Exception("Email already exists");
            }

            var isPhoneNumberExist = await _userRepo.GetAll().AnyAsync(x => x.PhonNumber == request.PhonNumber);
            if (isPhoneNumberExist)
            {
                throw new Exception("Phone number already exists");
            }
        }
    }
}
