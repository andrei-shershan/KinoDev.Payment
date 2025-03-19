using System.Threading.Tasks;
using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Payment.Infrastructure.Services;
using KinoDev.Payment.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace KinoDev.Payment.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly IMongoDbService _mongoDbService;

    public PaymentsController(IStripeService stripeService, IMongoDbService cosmosDbService)
    {
        _stripeService = stripeService;
        _mongoDbService = cosmosDbService;
    }

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntentAsync([FromBody] CreatePaymentIntentModel model)
    {
        System.Console.WriteLine("CreatePaymentIntentAsync");
        var paymentIntent = await  _stripeService.CreatePaymentIntentAsync(model.Amount, model.Metadata, model.Currency);
        if (paymentIntent != null)
        {
            System.Console.WriteLine("PaymentIntent: ");
            // Save payment intent to MongoDB
            await _mongoDbService.Foo(paymentIntent.ClientSecret);

            System.Console.WriteLine("PaymentIntent saved to MongoDB");
            return Ok(paymentIntent);
        }
        return BadRequest();
    }


    [HttpGet("hello")]
    [AllowAnonymous]
    public async Task<IActionResult> Hello()
    {
        await _stripeService.CreatePaymentIntentAsync(333, new Dictionary<string, string>(), "usd");
        return Ok("Hello from PaymentsController");
    }
}
