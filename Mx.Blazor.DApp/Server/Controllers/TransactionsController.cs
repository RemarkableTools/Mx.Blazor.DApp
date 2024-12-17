using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mx.Blazor.DApp.Services;
using Mx.Blazor.DApp.Shared;

namespace Mx.Blazor.DApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController(
    ITransactionsService transactionsService) : ControllerBase
{
    // generate wallet and save it to the local disk
    [HttpPost("generate-wallet/{shardId}")]
    public IActionResult GenerateWallet(uint shardId)
    {
        var address = transactionsService.GenerateWallet(shardId);
        if (!string.IsNullOrEmpty(address))
        {
            return Ok(address);
        }

        return BadRequest();
    }

    // schedule transactions for execution
    [HttpPost("schedule-transactions")]
    public async Task<IActionResult> ScheduleTransactions([FromBody] TransactionsRequest request)
    {
        await transactionsService.ScheduleTransactions(request);

        return Ok();
    }
}
