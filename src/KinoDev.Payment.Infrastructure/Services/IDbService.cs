using KinoDev.Payment.Infrastructure.Models.PaymentIntents;

namespace KinoDev.Payment.Infrastructure.Services
{
    public interface IDbService
    {
        Task SavePaymentIntentAsync(GenericPaymentIntent paymentIntent);
    }
}