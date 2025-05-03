using KinoDev.Payment.Infrastructure.MediatR.Commands;
using KinoDev.Shared.DtoModels.PaymentIntents;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace KinoDev.Payments.UnitTests.Controllers.PaymentControllers;

public class CompletePaymentAsyncTests : PaymentsControllerTestsBase
{
    [Fact]
    public async Task CompletePaymentAsync_WhenSuccessful_ReturnsOkResult()
    {
        // Arrange
        var paymentIntentId = "pi_123";
        var expectedResult = new GenericPaymentIntent { PaymentIntentId = paymentIntentId };
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CompletePaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);


        // Act
        var result = await _controller.CompletePaymentAsync(paymentIntentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public async Task CompletePaymentAsync_WhenResultIsNull_ReturnsBadRequest()
    {
        // Arrange
        var paymentIntentId = "pi_123";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CompletePaymentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _controller.CompletePaymentAsync(paymentIntentId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}