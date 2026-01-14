using Application.Services.ClientUserService;
using Application.Services.ClientUserService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientUsersController : ControllerBase
    {
        private readonly IClientUserService _clientUserService;
        public ClientUsersController(IClientUserService clientUserService)
        {
            _clientUserService = clientUserService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterClientUser([FromBody] ClientUserRegistrationRequest request)
        {
            await _clientUserService.ClientUserRegistration(request);
            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpGet("MyAccount")]
        public async Task<IActionResult> GetMyAccount()
        {
            var response = await _clientUserService.GetClientUserAccount();
            return Ok(response);
        }

        [Authorize(Roles = "User")]
        [HttpPost("UpdateMyAccount")]
        public async Task<IActionResult> UpdateClientUserAccount([FromBody] ClientUserRegistrationRequest request)
        {
            await _clientUserService.UpdateClientUserAccount(request);
            return Ok();
        }
    }
}
