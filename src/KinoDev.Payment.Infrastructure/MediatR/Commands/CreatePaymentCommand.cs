using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Shared.DtoModels.PaymentIntents;
using KinoDev.Shared.Enums;
using MediatR;

namespace KinoDev.Payment.Infrastructure.MediatR.Commands
{
    public class CreatePaymentCommand : IRequest<string>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, string>
    {
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly IDbService _dbService;

        public CreatePaymentCommandHandler(IPaymentProviderService paymentProviderService, IDbService dbService)
        {
            _paymentProviderService = paymentProviderService;
            _dbService = dbService;
        }

        public async Task<string> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentIntent = await _paymentProviderService.CreatePaymentIntentAsync(request.Amount, request.Metadata, request.Currency);
            if (paymentIntent == null)
            {
                return null;
            }

            await _dbService.SavePaymentIntentAsync(new GenericPaymentIntent()
            {
                PaymentProvider = _paymentProviderService.GetCurrentPaymentProvider(),                
                PaymentIntentId = paymentIntent.PaymentIntentId,
                Amount = paymentIntent.Amount,
                ClientSecret = paymentIntent.ClientSecret,
                Currency = paymentIntent.Currency,
                Metadata = paymentIntent.Metadata,
                State = paymentIntent.State,
                OrderId = request.OrderId
            });

            return paymentIntent.ClientSecret;
        }
    }
}
