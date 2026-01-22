namespace Application.Services.ChatService.DTOs
{
    public class SendChatMessageRequest
    {
        public string Message { get; set; }
        public int RecieverId { get; set; }
        public int? ChatId { get; set; }
    }
}
