using KinoDev.Payment.Infrastructure.Models.PaymentIntents;

namespace KinoDev.Payment.Infrastructure.Services
{
    public interface IDbService
    {
        Task SavePaymentIntentAsync(GenericPaymentIntent paymentIntent);

        Task<GenericPaymentIntent> GetPaymentIntentAsync(string paymentIntentId);

        Task<IEnumerable<GenericPaymentIntent>> GetOrderPaymentIntentsAsync(Guid orderId);

        Task UpdatePaymentIntentStateAsync(string paymentIntentId, string state);
    }
}