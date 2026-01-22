namespace Application.Services.ChatService.DTOs
{
    public class GetChatMessageResponse
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
