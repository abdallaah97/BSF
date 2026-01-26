using Application.Repositories;
using Application.Services.CurrentUserService;
using Application.Services.FileService;
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
        private readonly IFileService _fileService;
        public ServiceProviderService(IGenericRepository<ServiceProvider> serviceProviderRepo, IGenericRepository<User> userRepo, IGenericRepository<Role> roleRepo, ICurrentUserService currentUserService, IFileService fileService)
        {
            _serviceProviderRepo = serviceProviderRepo;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _currentUserService = currentUserService;
            _fileService = fileService;
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
                ServiceCategoryId = serviceProvider.ServiceCategoryId,
                IsAvailable = serviceProvider.IsAvailable,
                PersonalPhoto = serviceProvider.User.PersonalPhoto
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
                ServiceCategoryId = request.ServiceCategoryId,
                IsAvailable = false
            };

            await _serviceProviderRepo.InsertAsync(serviceProvider);
            await _serviceProviderRepo.SaveChangesAsync();
        }

        public async Task UpdateServiceProviderAccount(UpdateServiceProviderRequest request)
        {
            var userId = _currentUserService.UserId;

            await RegistrationValidation(request, userId.Value);

            var user = await _userRepo.GetByIdAsync(userId.Value);
            var serviceProvider = await _serviceProviderRepo.GetAll().FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (serviceProvider == null)
            {
                throw new Exception("Service provider user not found");
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.Name = request.Name;
            user.Email = request.Email;
            user.PhonNumber = request.PhonNumber;

            if (request.DeletePhoto)
            {
                _fileService.DeleteFile(user.PersonalPhoto);
                user.PersonalPhoto = null;
            }

            if (request.PersonalPhoto != null)
            {
                _fileService.DeleteFile(user.PersonalPhoto);
                user.PersonalPhoto = await _fileService.SaveFileAsync(request.PersonalPhoto, "Users");
            }

            _userRepo.Update(user);
            await _userRepo.SaveChangesAsync();

            serviceProvider.ServiceCategoryId = request.ServiceCategoryId;
            serviceProvider.IsAvailable = request.IsAvailable;
            _serviceProviderRepo.Update(serviceProvider);
            await _serviceProviderRepo.SaveChangesAsync();

        }


        private async Task RegistrationValidation(ServiceProviderRegistrationRequest request, int? userId = null)
        {
            if (userId == null)
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
            else
            {
                var isEmailExist = await _userRepo.GetAll().AnyAsync(x => x.Email == request.Email && x.Id != userId);
                if (isEmailExist)
                {
                    throw new Exception("Email already exists");
                }

                var isPhoneNumberExist = await _userRepo.GetAll().AnyAsync(x => x.PhonNumber == request.PhonNumber && x.Id != userId);
                if (isPhoneNumberExist)
                {
                    throw new Exception("Phone number already exists");
                }
            }

        }
    }
}
