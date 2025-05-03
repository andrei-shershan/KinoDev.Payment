using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Shared.Constants;
using MediatR;

namespace KinoDev.Payment.Infrastructure.MediatR.Commands
{
    public class CancelPendingOrderPaymentsCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
    }

    public class CancelPendingOrderPaymentsCommandHandler : IRequestHandler<CancelPendingOrderPaymentsCommand, bool>
    {
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly IDbService _dbService;

        public CancelPendingOrderPaymentsCommandHandler(IPaymentProviderService paymentProviderService, IDbService dbService)
        {
            _paymentProviderService = paymentProviderService;
            _dbService = dbService;
        }

        public async Task<bool> Handle(CancelPendingOrderPaymentsCommand request, CancellationToken cancellationToken)
        {
            var payments = await _dbService.GetOrderPaymentIntentsAsync(request.OrderId);
            if (payments != null)
            {
                // TODO: Review this loigic
                foreach (var payment in payments)
                {
                    await _paymentProviderService.CancelPaymentAsync(payment.PaymentIntentId);
                    await _dbService.UpdatePaymentIntentStateAsync(payment.PaymentIntentId, PaymentStates.Canceled);
                }

                return true;
            }

            return false;
        }
    }
}