namespace Application.Services.FirebaseService.DTOs
{
    public class RegisterFirebaseTokenRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
