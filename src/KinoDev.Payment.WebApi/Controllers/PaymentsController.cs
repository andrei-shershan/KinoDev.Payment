using KinoDev.Payment.Infrastructure.Services;
using KinoDev.Payment.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoDev.Payment.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IStripeService _stripeService;

    public PaymentsController(IStripeService stripeService)
    {
        _stripeService = stripeService;
    }

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntentAsync([FromBody] CreatePaymentIntentModel model)
    {
        System.Console.WriteLine("************* CreatePaymentIntentAsync " + model.Metadata.Count());
        var clientSecret = await _stripeService.CreatePaymentIntentAsync(model.Amount, model.Metadata, model.Currency);
        if (clientSecret != null)
        {
            return Ok(clientSecret);
        }
        return BadRequest();
    }

    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok("Hello World");
    }
}
