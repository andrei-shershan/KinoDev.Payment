using KinoDev.Payment.Infrastructure.MediatR.Queries;
using KinoDev.Shared.DtoModels.PaymentIntents;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KinoDev.Payments.UnitTests.Controllers.PaymentControllers;

public class GetPaymentAsyncTests : PaymentsControllerTestsBase
{
    [Fact]
    public async Task GetPaymentAsync_WhenPaymentExists_ReturnsOkResult()
    {
        // Arrange
        var paymentIntentId = "pi_123";
        var expectedResult = new GenericPaymentIntent { PaymentIntentId = paymentIntentId };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetPaymentIntentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetPaymentAsync(paymentIntentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public async Task GetPaymentAsync_WhenPaymentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var paymentIntentId = "pi_123";

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetPaymentIntentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        // Act
        var result = await _controller.GetPaymentAsync(paymentIntentId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}