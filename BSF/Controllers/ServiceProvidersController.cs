using Application.Services.ServiceProviderService;
using Application.Services.ServiceProviderService.DTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProvidersController : ControllerBase
    {
        private readonly IServiceProviderService _serviceProviderService;
        public ServiceProvidersController(IServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterServiceProvider([FromBody] ServiceProviderRegistrationRequest request)
        {
            await _serviceProviderService.ServiceProviderRegistration(request);
            return Ok();
        }

        [Authorize(Roles = "ServiceProvider")]
        [HttpGet("MyAccount")]
        public async Task<IActionResult> GetMyAccount()
        {
            var response = await _serviceProviderService.GetServiceProviderAccount();
            return Ok(response);
        }
    }
}
