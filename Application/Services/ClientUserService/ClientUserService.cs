using Application.Repositories;
using Application.Services.ClientUserService.DTOs;
using Application.Services.CurrentUserService;
using Application.Services.FileService;
using Domain.Entittes;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.ClientUserService
{
    public class ClientUserService : IClientUserService
    {
        private readonly IGenericRepository<ClientUser> _clientUserRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<Role> _roleRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        public ClientUserService(IGenericRepository<ServiceProvider> serviceProviderRepo, IGenericRepository<User> userRepo, IGenericRepository<Role> roleRepo, IGenericRepository<ClientUser> clientUserRepo, ICurrentUserService currentUserService, IFileService fileService)
        {
            _clientUserRepo = clientUserRepo;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _currentUserService = currentUserService;
            _fileService = fileService;
        }

        public async Task ClientUserRegistration(ClientUserRegistrationRequest request)
        {
            await RegistrationValidation(request);

            var clientUserRole = await _roleRepo.GetAll().FirstOrDefaultAsync(x => x.Code == SytemRole.User);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PhonNumber = request.PhonNumber,
                RoleId = clientUserRole.Id
            };

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, request.Password);

            await _userRepo.InsertAsync(user);
            await _userRepo.SaveChangesAsync();

            var clientUser = new ClientUser
            {
                UserId = user.Id,
                BarthDate = request.BirthDate
            };

            await _clientUserRepo.InsertAsync(clientUser);
            await _clientUserRepo.SaveChangesAsync();
        }

        public async Task<GetClientUserAccountResponse> GetClientUserAccount()
        {
            var userId = _currentUserService.UserId;

            var clientUser = await _clientUserRepo.GetAll()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (clientUser == null)
            {
                throw new Exception("Client user not found");
            }

            var response = new GetClientUserAccountResponse
            {
                Id = clientUser.Id,
                Name = clientUser.User.Name,
                Email = clientUser.User.Email,
                PhonNumber = clientUser.User.PhonNumber,
                BirthDate = clientUser.BarthDate,
                PersonalPhoto = clientUser.User.PersonalPhoto
            };

            return response;
        }

        public async Task UpdateClientUserAccount(UpdateClientUserRequest request)
        {
            var userId = _currentUserService.UserId;

            await RegistrationValidation(request, userId);

            var user = await _userRepo.GetByIdAsync(userId.Value);
            var clientUser = await _clientUserRepo.GetAll().FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (clientUser == null)
            {
                throw new Exception("Client user not found");
            }

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

            clientUser.BarthDate = request.BirthDate;
            _clientUserRepo.Update(clientUser);
            await _clientUserRepo.SaveChangesAsync();

        }

        private async Task RegistrationValidation(ClientUserRegistrationRequest request, int? id = null)
        {
            if (id == null)
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
                var isEmailExist = await _userRepo.GetAll().AnyAsync(x => x.Email == request.Email && x.Id != id.Value);
                if (isEmailExist)
                {
                    throw new Exception("Email already exists");
                }

                var isPhoneNumberExist = await _userRepo.GetAll().AnyAsync(x => x.PhonNumber == request.PhonNumber && x.Id != id.Value);
                if (isPhoneNumberExist)
                {
                    throw new Exception("Phone number already exists");
                }
            }

        }
    }
}
