using KinoDev.Payment.Infrastructure.MediatR.Commands;
using KinoDev.Payment.WebApi.Models;
using KinoDev.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.Payments.UnitTests.Controllers.PaymentControllers
{
    public class CreatePaymentAsyncTests : PaymentsControllerTestsBase
    {
        [Fact]
        public async Task ValidModel_ReturnsOkWithClientSecret()
        {
            // Arrange
            var model = new CreatePaymentModel
            {
                OrderId = Guid.NewGuid(),
                Amount = 100.50m,
                Metadata = new Dictionary<string, string>()
            };
            var expectedClientSecret = "client_secret_test";

            _mediatorMock.Setup(x => x.Send(
                It.Is<CreatePaymentCommand>(cmd => 
                    cmd.OrderId == model.OrderId && 
                    cmd.Amount == model.Amount &&
                    cmd.Currency == Currency.USD),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedClientSecret);

            // Act
            var result = await _controller.CreatePaymentAsync(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedClientSecret, returnValue);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CreatePaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task WhenMediatrReturnsInvalidValue_ReturnsBadRequest(string returnValue)
        {
            // Arrange
            var model = new CreatePaymentModel
            {
                OrderId = Guid.NewGuid(),
                Amount = 100.50m,
                Metadata = new Dictionary<string, string>()
            };

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreatePaymentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnValue);

            // Act
            var result = await _controller.CreatePaymentAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _mediatorMock.Verify(x => x.Send(It.IsAny<CreatePaymentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}