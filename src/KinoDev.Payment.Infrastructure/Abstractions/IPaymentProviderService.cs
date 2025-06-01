using KinoDev.Shared.DtoModels.PaymentIntents;
using KinoDev.Shared.Enums;

namespace KinoDev.Payment.Infrastructure.Abstractions
{
    public interface IPaymentProviderService
    {
        Task<GenericPaymentIntent?> CreatePaymentIntentAsync(decimal amount, Dictionary<string, string>? metadata, Currency currency);
        
        Task<bool> ConfirmPaymentAsync(string paymentIntentId);
        
        Task<bool> CancelPaymentAsync(string paymentIntentId);

        Task<GenericPaymentIntent?> GetPaymentIntentAsync(string paymentIntentId);

        Task<bool> CancelPaymentIntentAsync(string paymentIntentId);

        PaymentProvider GetCurrentPaymentProvider();
    }
}