namespace KinoDev.Payment.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency = "usd");
    }

    public class PaymentService : IPaymentService
    {
        private readonly IStripeService _stripeService;
        // private readonly IMongoDbService _mongoDbService;

        public PaymentService(IStripeService stripeService )
        //  IMongoDbService mongoDbService)
        {
            _stripeService = stripeService;
            // _mongoDbService = mongoDbService;
        }

        public async Task<string> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency = "usd")
        {
            // Create payment intent through Stripe
            var paymentIntent = await _stripeService.CreatePaymentIntentAsync(amount, metadata, currency);

            // await _mongoDbService.SavePaymentIntentAsync(paymentIntent);

            return paymentIntent.ClientSecret;
        }
    }
}
