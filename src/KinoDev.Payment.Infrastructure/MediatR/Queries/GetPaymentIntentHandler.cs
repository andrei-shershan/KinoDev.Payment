using KinoDev.Payment.Infrastructure.Models.PaymentIntents;
using KinoDev.Payment.Infrastructure.Services;
using MediatR;

namespace KinoDev.Payment.Infrastructure.MediatR.Queries
{
    public class GetPaymentIntentQuery : IRequest<GenericPaymentIntent>
    {
        public string PaymentIntentId { get; set; }
    }

    public class GetPaymentIntentHandler : IRequestHandler<GetPaymentIntentQuery, GenericPaymentIntent>
    {
        private readonly IPaymentProviderService _paymentProviderService;

        public GetPaymentIntentHandler(IPaymentProviderService paymentProviderService)
        {
            _paymentProviderService = paymentProviderService;
        }

        public async Task<GenericPaymentIntent> Handle(GetPaymentIntentQuery request, CancellationToken cancellationToken)
        {
            var paymentIntent = await _paymentProviderService.GetPaymentIntentAsync(request.PaymentIntentId);
            if (paymentIntent == null)
            {
                return null;
            }

            return paymentIntent;
        }
    }
}