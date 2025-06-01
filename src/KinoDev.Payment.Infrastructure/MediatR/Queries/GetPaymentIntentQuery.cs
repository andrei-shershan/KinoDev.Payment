using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Shared.DtoModels.PaymentIntents;
using MediatR;

namespace KinoDev.Payment.Infrastructure.MediatR.Queries
{
    public class GetPaymentIntentQuery : IRequest<GenericPaymentIntent>
    {
        public string? PaymentIntentId { get; set; }
    }

    public class GetPaymentIntentHandler : IRequestHandler<GetPaymentIntentQuery, GenericPaymentIntent?>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public GetPaymentIntentHandler(IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public async Task<GenericPaymentIntent?> Handle(GetPaymentIntentQuery request, CancellationToken cancellationToken)
        {
            return await _paymentProviderService.GetPaymentIntentAsync(request.PaymentIntentId);
        }
    }
}