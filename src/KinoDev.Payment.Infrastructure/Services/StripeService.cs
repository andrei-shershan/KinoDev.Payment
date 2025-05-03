using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Payment.Infrastructure.Constants;
using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Shared.Constants;
using KinoDev.Shared.DtoModels.PaymentIntents;
using KinoDev.Shared.Enums;
using KinoDev.Shared.Extensions;
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

        public async Task<GenericPaymentIntent> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, Currency currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Convert to cents
                Currency = currency.StringValue(),
                PaymentMethodTypes = new List<string>
                {
                    StripeConstants.PaymentMethods.Card,
                },
                Metadata = metadata?.Count() > 0 ? metadata : null
            };

            var service = new PaymentIntentService();
            try
            {
                var stripPaymentIntent = await service.CreateAsync(options);

                return new GenericPaymentIntent()
                {
                    PaymentIntentId = stripPaymentIntent.Id,
                    Amount = stripPaymentIntent.Amount / 100,
                    Currency = Enum.Parse<Currency>(stripPaymentIntent.Currency, ignoreCase: true), // TODO: Check if this is correct
                    ClientSecret = stripPaymentIntent.ClientSecret,
                    PaymentProvider = PaymentProvider.Stripe,
                    Metadata = stripPaymentIntent.Metadata
                };
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                return null;
            }

        }

        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            try
            {
                var paymentIntent = await service.GetAsync(paymentIntentId);
                return paymentIntent.Status == PaymentStates.Succeeded;
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                return false;
            }
        }

        public async Task<bool> CancelPaymentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.CancelAsync(paymentIntentId);
                return paymentIntent.Status == PaymentStates.Canceled;
            }
            catch (Exception ex)
            {
                // TODO: Log the exception
                return false;
            }
        }

        public PaymentProvider GetCurrentPaymentProvider()
        {
            return PaymentProvider.Stripe;
        }

        public async Task<GenericPaymentIntent> GetPaymentIntentAsync(string paymentIntentId)
        {
            var service = new PaymentIntentService();

            try
            {
                var paymentIntent = await service.GetAsync(paymentIntentId);

                return new GenericPaymentIntent()
                {
                    PaymentIntentId = paymentIntent.Id,
                    Amount = paymentIntent.Amount / 100,
                    Currency = Enum.Parse<Currency>(paymentIntent.Currency, ignoreCase: true), // TODO: Check if this is correct
                    ClientSecret = paymentIntent.ClientSecret,
                    PaymentProvider = PaymentProvider.Stripe,
                    Metadata = paymentIntent.Metadata,
                    State = paymentIntent.Status
                };
            }
            catch (Exception ex)
            {
                //TODO: Log the exception
                return null;
            }
        }

        public async Task<bool> CancelPaymentIntentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.CancelAsync(paymentIntentId);
                return paymentIntent.Status == PaymentStates.Canceled;
            }
            catch (StripeException ex)
            {
                // TODO: Log the exception
            }

            return false;
        }
    }
}