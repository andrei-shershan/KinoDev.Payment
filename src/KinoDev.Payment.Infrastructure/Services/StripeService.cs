using KinoDev.Payment.Infrastructure.Models;
using Microsoft.Extensions.Options;
using Stripe;

namespace KinoDev.Payment.Infrastructure.Services
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency = "usd");
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
            System.Console.WriteLine("KEY: " + _stripeSettings.SecretKey);
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency = "usd")
        {
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

            // System.Console.WriteLine("Creating payment intent, options: {@options}", options);
            System.Console.WriteLine("Creating payment intent, options//// " + options.Amount + " " + options.Currency + " " + options.PaymentMethodTypes + " " + options.Metadata);
            var service = new PaymentIntentService();
            return  await service.CreateAsync(options);
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