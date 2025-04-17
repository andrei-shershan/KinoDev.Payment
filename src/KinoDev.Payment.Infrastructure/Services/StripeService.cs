using KinoDev.Payment.Infrastructure.Constants;
using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Payment.Infrastructure.Models.PaymentIntents;
using Microsoft.Extensions.Options;
using Stripe;

namespace KinoDev.Payment.Infrastructure.Services
{
    public class StripeService : IPaymentProviderService
    {
        private readonly StripeSettings _stripeSettings;
        public StripeService(IOptions<StripeSettings> options)
        {
            _stripeSettings = options.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<GenericPaymentIntent> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Convert to cents
                Currency = currency,
                PaymentMethodTypes = new List<string>
                {
                    StripeConstants.PaymentMethods.Card,
                },
                Metadata = metadata?.Count() > 0 ? metadata : null
            };

            var service = new PaymentIntentService();
            var stripPaymentIntent = await service.CreateAsync(options);

            return new GenericPaymentIntent()
            {
                PaymentIntentId = stripPaymentIntent.Id,
                Amount = stripPaymentIntent.Amount / 100,
                Currency = stripPaymentIntent.Currency,
                ClientSecret = stripPaymentIntent.ClientSecret,
                PaymentProvider = Models.PaymentIntents.PaymentProvider.Stripe,
                Metadata = stripPaymentIntent.Metadata
            };
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

        public PaymentProvider GetCurrentPaymentProvider()
        {
            return PaymentProvider.Stripe;
        }

        public Task<GenericPaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            var paymentIntent = service.Get(paymentIntentId);

            return Task.FromResult(new GenericPaymentIntent()
            {
                PaymentIntentId = paymentIntent.Id,
                Amount = paymentIntent.Amount / 100,
                Currency = paymentIntent.Currency,
                ClientSecret = paymentIntent.ClientSecret,
                PaymentProvider = Models.PaymentIntents.PaymentProvider.Stripe,
                Metadata = paymentIntent.Metadata,
                State = paymentIntent.Status
            });
        }
    }
}