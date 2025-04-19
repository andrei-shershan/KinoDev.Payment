using KinoDev.Payment.Infrastructure.Models.PaymentIntents;
using KinoDev.Payment.Infrastructure.Services;
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
            var savedPaymentIntent = await _dbService.GetPaymentIntentAsync(request.PaymentIntentId);
            if (savedPaymentIntent == null)
            {
                return null;
            }

            var orderPaymentIntents = await _dbService.GetOrderPaymentIntentsAsync(savedPaymentIntent.OrderId);
            if (orderPaymentIntents == null)
            {
                return null;
            }

            var paymentIntentsForCancellation = orderPaymentIntents
                .Where(x => x.PaymentIntentId != request.PaymentIntentId);

            var cancellationTasks = paymentIntentsForCancellation
                .Select(x => _paymentProviderService.CancelPaymentIntentAsync(x.PaymentIntentId));

            var cancellationResults = await Task.WhenAll(cancellationTasks);

            var paymentIntentsUpdates = new Dictionary<string, string>();

            foreach (var pi in paymentIntentsForCancellation)
            {
                paymentIntentsUpdates.Add(pi.PaymentIntentId, "canceled");
            }

            paymentIntentsUpdates.Add(request.PaymentIntentId, "succeeded");

            foreach (var pi in paymentIntentsUpdates)
            {
                await _dbService.UpdatePaymentIntentStateAsync(pi.Key, pi.Value);
            }

            return await _dbService.GetPaymentIntentAsync(request.PaymentIntentId);
        }
    }
}