using Application.Services.ChatService.DTOs;

namespace Application.Services.ChatService
{
    public interface IChatService
    {
        Task SendChatMessage(SendChatMessageRequest request);
        Task<List<GetChatResponse>> GetUserChats();
        Task<List<GetChatMessageResponse>> GetMessagesByChatId(int chatId);
        Task UpdateMessageIsRead(int messageId);
    }
}
