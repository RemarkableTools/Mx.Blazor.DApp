using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Wallet;

namespace Mx.Blazor.DApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        [HttpPost("verify")]
        public IActionResult VerifyMessage(Message message)
        {
            var response = message.VerifyMessage();

            return Ok(response);
        }
    }
}
