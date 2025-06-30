using KinoDev.Payment.WebApi.Controllers;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace KinoDev.Payments.UnitTests.Controllers.PaymentControllers
{
    public abstract class PaymentsControllerTestsBase
    {
        protected readonly Mock<IMediator> _mediatorMock;
        protected readonly PaymentsController _controller;

        protected readonly Mock<ILogger<PaymentsController>> _loggerMock = new Mock<ILogger<PaymentsController>>();

        protected PaymentsControllerTestsBase()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PaymentsController(_mediatorMock.Object, _loggerMock.Object);
        }
    }
}
