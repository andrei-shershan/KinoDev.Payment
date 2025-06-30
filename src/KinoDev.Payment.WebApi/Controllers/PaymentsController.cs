using KinoDev.Payment.Infrastructure.MediatR.Commands;
using KinoDev.Payment.Infrastructure.MediatR.Queries;
using KinoDev.Payment.WebApi.Models;
using KinoDev.Shared.Enums;
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

    private ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePaymentAsync([FromBody] CreatePaymentModel model)
    {
        _logger.LogInformation("Creating payment for OrderId: {OrderId}, Amount: {Amount}, Metadata: {Metadata}",
            model.OrderId, model.Amount, model.Metadata);

        var result = await _mediator.Send(new CreatePaymentCommand()
        {
            OrderId = model.OrderId,
            Amount = model.Amount,
            Currency = Currency.USD, // TODO: USD for now, need to be dynamic
            Metadata = model.Metadata
        });

        if (string.IsNullOrWhiteSpace(result))
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompletePaymentAsync([FromRoute] string id)
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
    public async Task<IActionResult> GetPaymentAsync([FromRoute] string id)
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

    [HttpPost("order/{orderId}/cancel")]
    public async Task<IActionResult> CancelPendingOrderPayments([FromRoute] Guid orderId)
    {
        var result = await _mediator.Send(new CancelPendingOrderPaymentsCommand()
        {
            OrderId = orderId
        });

        if (result)
        {
            return Ok(result);
        }

        return BadRequest();
    }
}
