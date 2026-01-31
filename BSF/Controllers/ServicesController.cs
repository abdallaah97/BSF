using Application.Generic_DTOs;
using Application.Services.Service;
using Application.Services.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServicesService _servicesService;
        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }


        [Authorize(Roles = "ServiceProvider")]
        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService([FromForm] SaveServiceRequest request)
        {
            await _servicesService.CreateService(request);
            return Ok();
        }

        [Authorize(Roles = "ServiceProvider")]
        [HttpPost("UpdateService")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] SaveServiceRequest request)
        {
            await _servicesService.UpdateService(id, request);
            return Ok();
        }

        [Authorize(Roles = "ServiceProvider")]
        [HttpDelete("DeleteService")]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _servicesService.DeleteService(id);
            return Ok();
        }


        [Authorize(Roles = "ServiceProvider")]
        [HttpPost("GetMyServices")]
        public async Task<IActionResult> GetMyServices([FromBody] PaginationRequest request)
        {
            var response = await _servicesService.GetMyServices(request);
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("GetAllServices")]
        public async Task<IActionResult> GetAllServices([FromBody] GetServicesRequest request)
        {
            var response = await _servicesService.GetAllServices(request);
            return Ok(response);
        }
    }
}
