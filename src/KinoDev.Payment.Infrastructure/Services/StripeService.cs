using KinoDev.Payment.Infrastructure.Configuration;
using KinoDev.Payment.Infrastructure.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;

namespace KinoDev.Payment.Infrastructure.Services
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency = "usd");
        Task<bool> ConfirmPaymentAsync(string paymentIntentId);
        Task<bool> CancelPaymentAsync(string paymentIntentId);
    }

    public class StripeService : IStripeService
    {
        private readonly StripeSettings _stripeSettings;
        public StripeService(IOptions<StripeSettings> options)
        {
            _stripeSettings = options.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<string> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency = "usd")
        {
            System.Console.WriteLine("************* " + JsonConvert.SerializeObject(metadata));
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Convert to cents
                Currency = currency,
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Metadata = metadata?.Count() > 0 ? metadata : null
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            // Save to DB

            return paymentIntent.ClientSecret;
        }

        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);
            return paymentIntent.Status == "succeeded";
        }

        public async Task<bool> CancelPaymentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CancelAsync(paymentIntentId);
            return paymentIntent.Status == "canceled";
        }
    }
}