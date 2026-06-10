using Microsoft.AspNetCore.Mvc;
using SentinelAML.Application.Features.Customers.Commands.CreateCustomer;
using SentinelAML.Application.Features.Customers.Dtos;
using SentinelAML.Application.Features.Customers.Queries.GetCustomerById;
using SentinelAML.Application.Features.Customers.Queries.GetCustomers;

namespace SentinelAML.API.Controllers;

[Route("api/customers")]
public class CustomerController : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto request)
    {
        var result = await Mediator.Send(new CreateCustomerCommand(request));

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data }, new { id = result.Data });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetCustomersQuery());

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetCustomerByIdQuery(id));

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
}
