using KinoDev.Payment.Infrastructure.MediatR.Commands;
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

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntentAsync([FromBody] CreatePaymentIntentModel model)
    {
        var result = await _mediator.Send(new CreatePaymentCommand()
        {
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
}
