using KinoDev.Payment.WebApi.Controllers;
using MediatR;
using Moq;

namespace KinoDev.Payments.UnitTests.Controllers.PaymentControllers
{
    public abstract class PaymentsControllerTestsBase
    {
        protected readonly Mock<IMediator> _mediatorMock;
        protected readonly PaymentsController _controller;

        protected PaymentsControllerTestsBase()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PaymentsController(_mediatorMock.Object);
        }
    }
}
