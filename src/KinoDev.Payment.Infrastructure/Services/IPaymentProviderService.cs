using KinoDev.Payment.Infrastructure.Models.PaymentIntents;

namespace KinoDev.Payment.Infrastructure.Services
{
    public interface IPaymentProviderService
    {
        Task<GenericPaymentIntent> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string> metadata, string currency);
        Task<bool> ConfirmPaymentAsync(string paymentIntentId);
        Task<bool> CancelPaymentAsync(string paymentIntentId);

        PaymentProvider GetCurrentPaymentProvider();
    }
}