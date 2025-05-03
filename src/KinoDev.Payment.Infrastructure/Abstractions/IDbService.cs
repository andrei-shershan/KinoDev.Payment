using KinoDev.Shared.DtoModels.PaymentIntents;

namespace KinoDev.Payment.Infrastructure.Abstractions
{
    public interface IDbService
    {
        Task SavePaymentIntentAsync(GenericPaymentIntent paymentIntent);

        Task<GenericPaymentIntent> GetPaymentIntentAsync(string paymentIntentId);

        Task<IEnumerable<GenericPaymentIntent>> GetOrderPaymentIntentsAsync(Guid orderId);

        Task UpdatePaymentIntentStateAsync(string paymentIntentId, string state);
    }
}