using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Payment.Infrastructure.Constants;
using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Shared.Constants;
using KinoDev.Shared.DtoModels.PaymentIntents;
using KinoDev.Shared.Enums;
using KinoDev.Shared.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Stripe;

namespace KinoDev.Payment.Infrastructure.Services
{
    public class StripeService : IPaymentProviderService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly ILogger<StripeService> _logger;

        public StripeService(IOptions<StripeSettings> options, ILogger<StripeService> logger)
        {
            _stripeSettings = options.Value;
            _logger = logger;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<GenericPaymentIntent?> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, Currency currency)
        {
            _logger.LogInformation("Creating Stripe payment intent. Amount: {Amount}, Currency: {Currency}", amount, currency);
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
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to create Stripe payment intent. Amount: {Amount}, Currency: {Currency}", amount, currency);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating Stripe payment intent. Amount: {Amount}, Currency: {Currency}", amount, currency);
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
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to confirm Stripe payment. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error confirming Stripe payment. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
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
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to cancel Stripe payment. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error canceling Stripe payment. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
                return false;
            }
        }

        public PaymentProvider GetCurrentPaymentProvider()
        {
            return PaymentProvider.Stripe;
        }

        public async Task<GenericPaymentIntent?> GetPaymentIntentAsync(string paymentIntentId)
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
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to get Stripe payment intent. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting Stripe payment intent. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
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
                _logger.LogError(ex, "Failed to cancel Stripe payment intent. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error canceling Stripe payment intent. PaymentIntentId: {PaymentIntentId}", paymentIntentId);
                return false;
            }
        }
    }
}