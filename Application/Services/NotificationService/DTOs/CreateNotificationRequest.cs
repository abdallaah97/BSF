using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services.NotificationService.DTOs
{
    public class CreateNotificationRequest
    {
        public int UserId { get; set; }
        public int? OrderId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }
}


