using Application.Hubs;
using Application.Managers.Chat;
using Application.Repositories;
using Application.Services.ChatService.DTOs;
using Application.Services.CurrentUserService;
using Domain.Entittes;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IGenericRepository<Chat> _chatRepo;
        private readonly IGenericRepository<ChatMessage> _chatMessageRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IChatConnectionManager _chatConnectionManager;
        private readonly IHubContext<ChatHub> _chatHubContext;
        public ChatService(IGenericRepository<Chat> chatRepo, IGenericRepository<ChatMessage> chatMessageRepo, ICurrentUserService currentUserService, IHubContext<ChatHub> chatHubContext, IChatConnectionManager chatConnectionManager)
        {
            _chatRepo = chatRepo;
            _chatMessageRepo = chatMessageRepo;
            _currentUserService = currentUserService;
            _chatHubContext = chatHubContext;
            _chatConnectionManager = chatConnectionManager;
        }

        public async Task SendChatMessage(SendChatMessageRequest request)
        {
            var userId = _currentUserService.UserId.Value;
            var chat = new Chat();

            if (request.ChatId == null)
            {
                chat = new Chat
                {
                    FirstUserId = userId,
                    SecondUserId = request.RecieverId,
                    LastMessageDate = DateTime.UtcNow,
                };

                await _chatRepo.InsertAsync(chat);
                await _chatRepo.SaveChangesAsync();
            }
            else
            {
                chat = await _chatRepo.GetByIdAsync(request.ChatId.Value);
                chat.LastMessageDate = DateTime.UtcNow;

                _chatRepo.Update(chat);
                await _chatRepo.SaveChangesAsync();
            }

            var message = new ChatMessage
            {
                ChatId = chat.Id,
                SenderId = userId,
                ReciverId = request.RecieverId,
                IsRead = false,
                CreatedDate = DateTime.UtcNow,
                Message = request.Message,
            };

            if (_chatConnectionManager.TryGet(message.ReciverId.ToString(), out string connectionId))
            {
                await _chatHubContext.Clients.Client(connectionId)
                    .SendAsync("newMessage", message);
            }

            await _chatMessageRepo.InsertAsync(message);
            await _chatMessageRepo.SaveChangesAsync();
        }

        public async Task<List<GetChatResponse>> GetUserChats()
        {
            var userId = _currentUserService.UserId.Value;

            var chats = await _chatRepo.GetAll()
                .OrderByDescending(x => x.LastMessageDate)
                .Include(x => x.ChatMessages)
                .Include(x => x.SecondUser)
                .Include(x => x.FirstUser)
                .Where(x => x.FirstUserId == userId || x.SecondUserId == userId)
                .Select(x => new GetChatResponse
                {
                    Id = x.Id,
                    LastMessage = x.ChatMessages.OrderByDescending(x => x.CreatedDate).FirstOrDefault().Message,
                    FirstUserId = x.FirstUser.Id,
                    FirstUserName = x.FirstUser.Name,
                    SecondUserId = x.SecondUser.Id,
                    SecondUserName = x.SecondUser.Name,
                }).ToListAsync();

            return chats;
        }

        public async Task<List<GetChatMessageResponse>> GetMessagesByChatId(int chatId)
        {
            var messages = await _chatMessageRepo.GetAll()
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.ChatId == chatId)
                .Select(x => new GetChatMessageResponse
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    ReciverId = x.ReciverId,
                    Message = x.Message,
                    IsRead = x.IsRead,
                    CreatedDate = x.CreatedDate
                }).ToListAsync();

            return messages;
        }

        public async Task UpdateMessageIsRead(int messageId)
        {
            var message = await _chatMessageRepo.GetByIdAsync(messageId);
            message.IsRead = true;

            _chatMessageRepo.Update(message);
            await _chatMessageRepo.SaveChangesAsync();
        }
    }
}
