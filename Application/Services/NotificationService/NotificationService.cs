using Application.Generic_DTOs;
using Application.Repositories;
using Application.Services.CurrentUserService;
using Application.Services.NotificationService.DTOs;
using Domain.Entittes;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IGenericRepository<Notification> _notificationRepo;
        private readonly ICurrentUserService _currentUserService;
        public NotificationService(IGenericRepository<Notification> notificationRepo, ICurrentUserService currentUserService)
        {
            _notificationRepo = notificationRepo;
            _currentUserService = currentUserService;
        }

        public async Task CreateNotification(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                OrderId = request.OrderId,
                Title = request.Title,
                Message = request.Message,
                IsRead = false,
                CreatedData = DateTime.UtcNow,
            };

            await _notificationRepo.InsertAsync(notification);
            await _notificationRepo.SaveChangesAsync();
        }

        public async Task UpdateIsReaded(int notificationId)
        {
            var notification = await _notificationRepo.GetByIdAsync(notificationId);
            notification.IsRead = true;

            _notificationRepo.Update(notification);
            await _notificationRepo.SaveChangesAsync();
        }

        public async Task UpdateAllIsReaded()
        {
            var userId = _currentUserService.UserId.Value;

            var notifications = await _notificationRepo.GetAll().Where(x => x.UserId == userId && !x.IsRead).ToListAsync();
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _notificationRepo.Update(notification);
            }

            await _notificationRepo.SaveChangesAsync();
        }

        public async Task<PaginationResponse<GetNotificationResponse>> GetUserNotifications(PaginationRequest request)
        {
            var userId = _currentUserService.UserId.Value;

            var qurey = _notificationRepo.GetAll().OrderByDescending(x => x.CreatedData)
                .Where(x => x.UserId == userId)
                .Skip(request.PageSize * request.PageIndex)
                .Take(request.PageSize);

            var count = await qurey.CountAsync();

            var result = await qurey.Select(x => new GetNotificationResponse
            {
                Id = x.Id,
                Title = x.Title,
                Message = x.Message,
                OrderId = x.OrderId,
                IsRead = x.IsRead,
                CreatedData = x.CreatedData
            }).ToListAsync();

            return new PaginationResponse<GetNotificationResponse>
            {
                Items = result,
                Count = count
            };
        }
    }
}
