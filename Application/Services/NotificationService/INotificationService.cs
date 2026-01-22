using Application.Generic_DTOs;
using Application.Services.NotificationService.DTOs;

namespace Application.Services.NotificationService
{
    public interface INotificationService
    {
        Task CreateNotification(CreateNotificationRequest request);
        Task UpdateIsReaded(int notificationId);
        Task UpdateAllIsReaded();
        Task<PaginationResponse<GetNotificationResponse>> GetUserNotifications(PaginationRequest request);
    }
}
