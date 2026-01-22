namespace Application.Services.NotificationService.DTOs
{
    public class GetNotificationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? OrderId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedData { get; set; }
    }
}
