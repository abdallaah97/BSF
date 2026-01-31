namespace Application.Services.AuthService.DTOs
{
    public class ResetUserPasswordRequest
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }
}
