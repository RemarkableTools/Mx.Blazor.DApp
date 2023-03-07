using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Server.Services;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Helper;

namespace Mx.Blazor.DApp.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("verify")]
        public IActionResult VerifyMessage(SignableMessage message)
        {
            var response = _walletService.Verify(message);

            return Ok(response);
        }
    }
}
