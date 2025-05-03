using KinoDev.Payment.Infrastructure.MediatR.Commands;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace KinoDev.Payments.UnitTests.Controllers.PaymentControllers;

public class CancelPendingOrderPaymentsTests : PaymentsControllerTestsBase
{
    [Fact]
    public async Task CancelPendingOrderPayments_WhenSuccessful_ReturnsOkResult()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.Is<CancelPendingOrderPaymentsCommand>(cmd => 
            cmd.OrderId == orderId), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CancelPendingOrderPayments(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True(okResult.Value as bool?);
    }

    [Fact]
    public async Task CancelPendingOrderPayments_WhenFailed_ReturnsBadRequest()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.Is<CancelPendingOrderPaymentsCommand>(cmd => 
            cmd.OrderId == orderId), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.CancelPendingOrderPayments(orderId);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}