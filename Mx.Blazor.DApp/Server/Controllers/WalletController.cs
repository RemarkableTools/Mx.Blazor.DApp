using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Wallet;

namespace Mx.Blazor.DApp.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        [HttpPost("verify")]
        public IActionResult VerifyMessage(SignableMessage message)
        {
            var response = message.VerifyMessage();

            return Ok(response);
        }
    }
}
