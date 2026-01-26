using Application.Repositories;
using Application.Services.AuthService.DTOs;
using Application.Services.CurrentUserService;
using Domain.Entittes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<ServiceProvider> _serviceProviderRepository;
        private readonly IGenericRepository<ClientUser> _clientUserRepository;
        private readonly IGenericRepository<RefershToken> _refershTokenRepository;
        private readonly IGenericRepository<FirebaseToken> _firebaseTokenRepository;
        public AuthService(IGenericRepository<User> userRepository, IGenericRepository<RefershToken> refershTokenRepository, IConfiguration config, ICurrentUserService currentUserService, IGenericRepository<ServiceProvider> serviceProviderRepository, IGenericRepository<ClientUser> clientUserRepository, IGenericRepository<FirebaseToken> firebaseTokenRepository)
        {
            _userRepository = userRepository;
            _refershTokenRepository = refershTokenRepository;
            _config = config;
            _currentUserService = currentUserService;
            _serviceProviderRepository = serviceProviderRepository;
            _clientUserRepository = clientUserRepository;
            _firebaseTokenRepository = firebaseTokenRepository;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            // Eager loading
            var user = await _userRepository.GetAll()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Username.Trim().ToLower() || u.PhonNumber == request.Username.Trim());

            if (user == null)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<User>();
            var passwordResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var refershToken = new RefershToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            await _refershTokenRepository.InsertAsync(refershToken);
            await _refershTokenRepository.SaveChangesAsync();


            var result = new LoginResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhonNumber = user.PhonNumber,
                RoleCode = user.Role.Code,
                AccessToken = await GenerateAccessToken(user),
                RefreshToken = refershToken.Token,
            };

            return result;
        }

        public async Task<string> GenerateNewAccessToken(string refreshToken)
        {
            var token = await _refershTokenRepository.GetAll()
                .FirstOrDefaultAsync(t => t.Token == refreshToken);

            if (token == null || token.ExpiryDate < DateTime.UtcNow)
            {
                return null;
            }

            var user = await _userRepository.GetAll()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == token.UserId);

            return await GenerateAccessToken(user);
        }

        public async Task ChangeMyPassword(ChangeMyPasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value);

            var passwordHasher = new PasswordHasher<User>();
            var passwordResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.CurrentPassword);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                throw new Exception("Current password is incorrect.");
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new Exception("Confirm password is incorrect.");
            }

            user.Password = passwordHasher.HashPassword(user, request.NewPassword);
        }

        private async Task<string> GenerateAccessToken(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.PhonNumber),
                new Claim(ClaimTypes.Role, user.Role.Name),
            };

            var serviceProviderUser = await _serviceProviderRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (serviceProviderUser != null)
            {
                claims.Add(new Claim("ServiceProviderId", serviceProviderUser.Id.ToString()));
            }
            else
            {
                var clientUser = await _clientUserRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (clientUser != null)
                {
                    claims.Add(new Claim("clientUserId", clientUser.Id.ToString()));
                }
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);

        }

        private string GenerateRefreshToken()
        {
            var random = new byte[64];
            RandomNumberGenerator.Fill(random);
            return Convert.ToBase64String(random);
        }

        public async Task RegisterFirbaseToken(RegisterFirebaseTokenRequest request)
        {
            var userId = _currentUserService.UserId.Value;

            var existingToken = await _firebaseTokenRepository.GetAll()
                .FirstOrDefaultAsync(t => t.Token == request.Token && t.UserId == userId);

            if (existingToken == null)
            {
                var firebaseToken = new FirebaseToken
                {
                    UserId = userId,
                    Token = request.Token
                };
                await _firebaseTokenRepository.InsertAsync(firebaseToken);
                await _firebaseTokenRepository.SaveChangesAsync();
            }
        }
    }
}
