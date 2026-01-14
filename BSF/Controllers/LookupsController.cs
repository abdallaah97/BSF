using Application.Services.LookupService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BSF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        private readonly ILookupService _lookupService;
        public LookupsController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [AllowAnonymous]
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _lookupService.GetAllServiceCategories();
            return Ok(categories);
        }
    }
}
