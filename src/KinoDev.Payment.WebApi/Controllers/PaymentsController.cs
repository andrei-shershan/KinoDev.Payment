using KinoDev.Payment.Infrastructure.MediatR.Commands;
using KinoDev.Payment.Infrastructure.MediatR.Queries;
using KinoDev.Payment.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.Payment.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePaymentAsync([FromBody] CreatePaymentModel model)
    {
        var result = await _mediator.Send(new CreatePaymentCommand()
        {
            OrderId = model.OrderId,
            Amount = model.Amount,
            Currency = model.Currency,
            Metadata = model.Metadata
        });

        if (!string.IsNullOrWhiteSpace(result))
        {
            return Ok(result);
        }

        return BadRequest();
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompletePaymentAsync(string id)
    {
        var result = await _mediator.Send(new CompletePaymentCommand()
        {
            PaymentIntentId = id
        });

        if (result != null)
        {
            return Ok(result);
        }

        return BadRequest();
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentAsync(string id)
    {
        var result = await _mediator.Send(new GetPaymentIntentQuery()
        {
            PaymentIntentId = id
        });

        if (result != null)
        {
            return Ok(result);
        }

        return NotFound();
    }
}
