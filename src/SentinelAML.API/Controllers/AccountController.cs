using Microsoft.AspNetCore.Mvc;
using SentinelAML.Application.Features.Accounts.Commands.CloseAccount;
using SentinelAML.Application.Features.Accounts.Commands.CreateAccount;
using SentinelAML.Application.Features.Accounts.Dtos;
using SentinelAML.Application.Features.Accounts.Queries.GetAccountById;
using SentinelAML.Application.Features.Accounts.Queries.GetAccountsByCustomer;

namespace SentinelAML.API.Controllers;

[Route("api")]
public class AccountController : ApiControllerBase
{
    [HttpPost("accounts")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateAccountDto request)
    {
        var result = await Mediator.Send(new CreateAccountCommand(request));

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
            {
                return NotFound(new { errors = result.Errors });
            }

            return BadRequest(new { errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, new { id = result.Data });
    }

    [HttpGet("accounts/{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetAccountByIdQuery(id));

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
            {
                return NotFound(new { errors = result.Errors });
            }

            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    [HttpGet("customers/{customerId:guid}/accounts")]
    [ProducesResponseType(typeof(IReadOnlyList<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var result = await Mediator.Send(new GetAccountsByCustomerQuery(customerId));

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
            {
                return NotFound(new { errors = result.Errors });
            }

            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    [HttpPut("accounts/{id:guid}/close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Close(Guid id)
    {
        var result = await Mediator.Send(new CloseAccountCommand(id));

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Contains("not found", StringComparison.OrdinalIgnoreCase)))
            {
                return NotFound(new { errors = result.Errors });
            }

            return BadRequest(new { errors = result.Errors });
        }

        return Ok();
    }
}
