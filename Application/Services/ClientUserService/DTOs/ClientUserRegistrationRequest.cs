namespace Application.Services.ClientUserService.DTOs
{
    public class ClientUserRegistrationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhonNumber { get; set; }
        public string? Password { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
