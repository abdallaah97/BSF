using Application.Services.AuthService.DTOs;

namespace Application.Services.AuthService
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<string> GenerateNewAccessToken(string refreshToken);
        Task ChangeMyPassword(ChangeMyPasswordRequest request);
        Task RegisterFirbaseToken(RegisterFirebaseTokenRequest request);
    }
}
