using Hyper.Application.Features.Points.Jobs;
using Hyper.Domain.Entities.Customers;
using Hyper.Domain.Entities.Customers.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.Domain.Repository;
using System.Linq.Expressions;

namespace Hyper.Application.Tests.Features.Points;

public class ExpirePointsJobTests
{
    [Fact]
    public async Task Run_WhenExpiredTransactionsExist_ShouldMarkThemAsExpired()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expiredTransaction1 = new CustomerTransaction
        {
            Id = 1,
            ExpirationDate = now.AddDays(-1), // Expired yesterday
            IsExpired = false,
            IsSpent = false,
            TransactionType = CustomerTransactionType.Credit,
            Credit = 100
        };

        var expiredTransaction2 = new CustomerTransaction
        {
            Id = 2,
            ExpirationDate = now.AddHours(-1), // Expired 1 hour ago
            IsExpired = false,
            IsSpent = false,
            TransactionType = CustomerTransactionType.Credit,
            Credit = 200
        };

        var expiredTransactions = new List<CustomerTransaction> { expiredTransaction1, expiredTransaction2 };

        var queryRepoMock = new Mock<IQueryRepository<CustomerTransaction, long>>();
        queryRepoMock.Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<CustomerTransaction, bool>>>(),
                null,
                null,
                null))
            .ReturnsAsync(expiredTransactions);

        var cmdRepoMock = new Mock<ICommandRepository<CustomerTransaction, long>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        cmdRepoMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);
        cmdRepoMock.Setup(x => x.Update(It.IsAny<CustomerTransaction>()));

        var loggerMock = new Mock<ILogger<ExpirePointsJob>>();

        var job = new ExpirePointsJob(queryRepoMock.Object, cmdRepoMock.Object, loggerMock.Object);

        // Act
        await job.Run();

        // Assert
        Assert.True(expiredTransaction1.IsExpired);
        Assert.NotNull(expiredTransaction1.ExpiredDate);
        Assert.True(expiredTransaction2.IsExpired);
        Assert.NotNull(expiredTransaction2.ExpiredDate);
        cmdRepoMock.Verify(x => x.Update(expiredTransaction1), Times.Once);
        cmdRepoMock.Verify(x => x.Update(expiredTransaction2), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Run_WhenNoExpiredTransactions_ShouldNotUpdateAnything()
    {
        // Arrange
        var queryRepoMock = new Mock<IQueryRepository<CustomerTransaction, long>>();
        queryRepoMock.Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<CustomerTransaction, bool>>>(),
                null,
                null,
                null))
            .ReturnsAsync(new List<CustomerTransaction>());

        var cmdRepoMock = new Mock<ICommandRepository<CustomerTransaction, long>>();
        var loggerMock = new Mock<ILogger<ExpirePointsJob>>();

        var job = new ExpirePointsJob(queryRepoMock.Object, cmdRepoMock.Object, loggerMock.Object);

        // Act
        await job.Run();

        // Assert
        cmdRepoMock.Verify(x => x.Update(It.IsAny<CustomerTransaction>()), Times.Never);
        cmdRepoMock.Verify(x => x.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Run_WhenTransactionIsAlreadyExpired_ShouldNotProcessIt()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var alreadyExpiredTransaction = new CustomerTransaction
        {
            Id = 1,
            ExpirationDate = now.AddDays(-1),
            IsExpired = true, // Already expired
            IsSpent = false,
            TransactionType = CustomerTransactionType.Credit,
            Credit = 100
        };

        var queryRepoMock = new Mock<IQueryRepository<CustomerTransaction, long>>();
        queryRepoMock.Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<CustomerTransaction, bool>>>(),
                null,
                null,
                null))
            .ReturnsAsync(new List<CustomerTransaction>()); // Query won't return already expired

        var cmdRepoMock = new Mock<ICommandRepository<CustomerTransaction, long>>();
        var loggerMock = new Mock<ILogger<ExpirePointsJob>>();

        var job = new ExpirePointsJob(queryRepoMock.Object, cmdRepoMock.Object, loggerMock.Object);

        // Act
        await job.Run();

        // Assert
        cmdRepoMock.Verify(x => x.Update(It.IsAny<CustomerTransaction>()), Times.Never);
    }

    [Fact]
    public async Task Run_WhenTransactionIsSpent_ShouldNotProcessIt()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var spentTransaction = new CustomerTransaction
        {
            Id = 1,
            ExpirationDate = now.AddDays(-1),
            IsExpired = false,
            IsSpent = true, // Already spent
            TransactionType = CustomerTransactionType.Credit,
            Credit = 100
        };

        var queryRepoMock = new Mock<IQueryRepository<CustomerTransaction, long>>();
        queryRepoMock.Setup(x => x.GetAllAsync(
                It.IsAny<CancellationToken>(),
                It.IsAny<Expression<Func<CustomerTransaction, bool>>>(),
                null,
                null,
                null))
            .ReturnsAsync(new List<CustomerTransaction>()); // Query won't return spent transactions

        var cmdRepoMock = new Mock<ICommandRepository<CustomerTransaction, long>>();
        var loggerMock = new Mock<ILogger<ExpirePointsJob>>();

        var job = new ExpirePointsJob(queryRepoMock.Object, cmdRepoMock.Object, loggerMock.Object);

        // Act
        await job.Run();

        // Assert
        cmdRepoMock.Verify(x => x.Update(It.IsAny<CustomerTransaction>()), Times.Never);
    }
}

