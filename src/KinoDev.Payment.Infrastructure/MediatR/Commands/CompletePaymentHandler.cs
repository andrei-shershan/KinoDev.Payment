using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Shared.Constants;
using KinoDev.Shared.DtoModels.PaymentIntents;
using MediatR;

namespace KinoDev.Payment.Infrastructure.MediatR.Commands
{

    public class CompletePaymentCommand : IRequest<GenericPaymentIntent>
    {
        public string PaymentIntentId { get; set; }
    }

    public class CompletePaymentHandler : IRequestHandler<CompletePaymentCommand, GenericPaymentIntent>
    {
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly IDbService _dbService;

        public CompletePaymentHandler(IPaymentProviderService paymentProviderService, IDbService dbService)
        {
            _paymentProviderService = paymentProviderService;
            _dbService = dbService;
        }

        public async Task<GenericPaymentIntent> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentIntent = await _dbService.GetPaymentIntentAsync(request.PaymentIntentId);
            if (paymentIntent == null)
            {
                return null;
            }

            await _dbService.UpdatePaymentIntentStateAsync(paymentIntent.PaymentIntentId, PaymentStates.Succeeded);

            return await _dbService.GetPaymentIntentAsync(request.PaymentIntentId);
        }
    }
}