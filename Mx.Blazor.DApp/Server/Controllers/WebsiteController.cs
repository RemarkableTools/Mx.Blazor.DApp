using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mx.Blazor.DApp.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WebsiteController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult TestAccess()
        {
            return Ok("YOU HAVE ACCESS!");
        }
    }
}
