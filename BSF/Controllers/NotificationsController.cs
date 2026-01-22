using Application.Generic_DTOs;
using Application.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSF.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("GetUserNotifications")]
        public async Task<IActionResult> GetUserNotifications([FromBody] PaginationRequest request)
        {
            var response = await _notificationService.GetUserNotifications(request);
            return Ok(response);
        }

        [HttpPost("UpdateIsReaded")]
        public async Task<IActionResult> UpdateIsReaded(int notificationId)
        {
            await _notificationService.UpdateIsReaded(notificationId);
            return Ok();
        }

        [HttpPost("UpdateAllIsReaded")]
        public async Task<IActionResult> UpdateAllIsReaded()
        {
            await _notificationService.UpdateAllIsReaded();
            return Ok();
        }
    }
}
