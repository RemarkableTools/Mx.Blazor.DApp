using System.Net;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Services;
using Mx.Blazor.DApp.Shared.Connection;
using HttpResponse = Mx.Blazor.DApp.Shared.Models.HttpResponse;

namespace Mx.Blazor.DApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController(IConnectionService connectionService) : ControllerBase
    {
        [HttpPost("verify")]
        public IActionResult Verify(AccessToken accessToken)
        {
            var response = connectionService.Verify(accessToken.Value);
            if (response == null)
                return BadRequest(
                    new HttpResponse()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "Access token could no be generated",
                        Error = "Token error"
                    }
                );

            return Ok(response);
        }
    }
}
