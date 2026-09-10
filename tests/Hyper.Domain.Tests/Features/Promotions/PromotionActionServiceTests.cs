//using Hyper.Domain.Entities.Customers;
//using Hyper.Domain.Entities.Customers.Enums;
//using Hyper.Domain.Entities.CustomerSegments.Data;
//using Hyper.Domain.Entities.Events;
//using Hyper.Domain.Entities.Events.Data;
//using Hyper.Domain.Entities.Plans.Data;
//using Hyper.Domain.Entities.Points;
//using Hyper.Domain.Entities.Points.Enums;
//using Hyper.Domain.Entities.Promotions;
//using Hyper.Domain.Entities.Promotions.Enums;
//using Hyper.Domain.Entities.Referal;
//using Hyper.Domain.Entities.Tenants.Data;
//using Hyper.Domain.Features.Points;
//using Hyper.Domain.Features.Promotions;
//using Hyper.Domain.Features.Rewards;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Neo.Domain.Features.Integrations;
//using Neo.Domain.Features.Sms;
//using Neo.Domain.Repository;

//namespace Hyper.Domain.Tests.Features.Promotions;

///// <summary>
///// تست‌های PromotionActionService
///// </summary>
//public class PromotionActionServiceTests
//{
//    private readonly Mock<ILogger<PromotionActionService>> _loggerMock;
//    private readonly Mock<IPointBudgetService> _pointBudgetServiceMock;
//    private readonly Mock<IPointLevelService> _pointLevelServiceMock;
//    private readonly Mock<ICommandRepository<CustomerTransaction, long>> _customerTransactionCmdRepoMock;
//    private readonly Mock<ICommandRepository<CustomerPointLevel, int>> _customerPointLevelCmdRepoMock;
//    private readonly Mock<IQueryRepository<EventLog, long>> _eventLogQueryRepoMock;
//    private readonly Mock<ICommandRepository<CustomerTenant, int>> _customerTenantCmdRepoMock;

//    public PromotionActionServiceTests()
//    {
//        _loggerMock = new Mock<ILogger<PromotionActionService>>();
//        _pointBudgetServiceMock = new Mock<IPointBudgetService>();
//        _pointLevelServiceMock = new Mock<IPointLevelService>();
//        _customerTransactionCmdRepoMock = new Mock<ICommandRepository<CustomerTransaction, long>>();
//        _customerPointLevelCmdRepoMock = new Mock<ICommandRepository<CustomerPointLevel, int>>();
//        _eventLogQueryRepoMock = new Mock<IQueryRepository<EventLog, long>>();
//        _customerTenantCmdRepoMock = new Mock<ICommandRepository<CustomerTenant, int>>();
//    }

//    #region SetPointLevel Tests

//    [Fact]
//    public async Task SetPointLevel_WhenPointLevelIdIsNull_ShouldLogWarningAndReturn()
//    {
//        // Arrange
//        var action = new PromotionAction
//        {
//            Id = 1,
//            ActionKind = PromotionActionKind.SetPointLevel,
//            PointLevelId = null,
//            AmountFormula = "0" // Required by ExtractValueAsync even though not used for SetPointLevel
//        };

//        var request = new PromotionProcessingRequest(1, 1, 0, 0, new Customer { Id = 1 }, null);
        
//        _advancedFormulaEvaluatorMock.Setup(x => x.EvaluateValueAsync(
//                It.IsAny<string>(),
//                It.IsAny<PromotionProcessingRequest>(),
//                It.IsAny<CancellationToken>()))
//            .ReturnsAsync((object?)0);

//        var service = CreateService();

//        // Act
//        await service.DoActionAsync(request, action, CancellationToken.None);

//        // Assert - Should not throw, just log warning
//        // Verify through logger if needed
//    }

//    [Fact]
//    public async Task SetPointLevel_WhenPointLevelExists_ShouldCreateNewCustomerPointLevel()
//    {
//        // Arrange
//        var pointLevel = new PointLevel { Id = 5, PointId = 1, Title = "VIP", Level = 5 };
//        var action = new PromotionAction
//        {
//            Id = 1,
//            ActionKind = PromotionActionKind.SetPointLevel,
//            PointLevelId = 5,
//            PointLevel = pointLevel,
//            AmountFormula = "0" // Required by ExtractValueAsync even though not used for SetPointLevel
//        };

//        var eventLog = new EventLog
//        {
//            Id = 1,
//            TenantId = 1,
//            EventTypeId = 1
//        };

//        var customerTenant = new CustomerTenant
//        {
//            Id = 1,
//            CustomerId = 1,
//            TenantId = 1
//        };

//        var request = new PromotionProcessingRequest(1, 1, 0, 0, new Customer { Id = 1 }, null);

//        _eventLogQueryRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<EventLog, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<EventLog>, System.Linq.IOrderedQueryable<EventLog>>>()))
//            .ReturnsAsync(eventLog);

//        _customerTenantCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTenant, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTenant>, System.Linq.IOrderedQueryable<CustomerTenant>>>()))
//            .ReturnsAsync(customerTenant);

//        _customerPointLevelCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerPointLevel, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerPointLevel>, System.Linq.IOrderedQueryable<CustomerPointLevel>>>()))
//            .ReturnsAsync((CustomerPointLevel?)null); // No existing level

//        var unitOfWorkMock = new Mock<IUnitOfWork>();
//        _customerPointLevelCmdRepoMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

//        _advancedFormulaEvaluatorMock.Setup(x => x.EvaluateValueAsync(
//                It.IsAny<string>(),
//                It.IsAny<PromotionProcessingRequest>(),
//                It.IsAny<CancellationToken>()))
//            .ReturnsAsync((object?)0);

//        var service = CreateService();

//        // Act
//        await service.DoActionAsync(request, action, CancellationToken.None);

//        // Assert
//        _customerPointLevelCmdRepoMock.Verify(x => x.Add(It.Is<CustomerPointLevel>(cpl => 
//            cpl.PointLevelId == 5 && 
//            cpl.CustomerTenantId == 1 && 
//            cpl.EventLogId == 1)), Times.Once);
//        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task SetPointLevel_WhenCustomerAlreadyHasLevel_ShouldNotCreateDuplicate()
//    {
//        // Arrange
//        var pointLevel = new PointLevel { Id = 5, PointId = 1, Title = "VIP", Level = 5 };
//        var action = new PromotionAction
//        {
//            Id = 1,
//            ActionKind = PromotionActionKind.SetPointLevel,
//            PointLevelId = 5,
//            PointLevel = pointLevel,
//            AmountFormula = "0" // Required by ExtractValueAsync even though not used for SetPointLevel
//        };

//        var existingLevel = new CustomerPointLevel
//        {
//            Id = 1,
//            CustomerTenantId = 1,
//            PointLevelId = 5,
//            EventLogId = 1
//        };

//        var eventLog = new EventLog { Id = 1, TenantId = 1 };
//        var customerTenant = new CustomerTenant { Id = 1, CustomerId = 1, TenantId = 1 };
//        var request = new PromotionProcessingRequest(1, 1, 0, 0, new Customer { Id = 1 }, null);

//        _eventLogQueryRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<EventLog, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<EventLog>, System.Linq.IOrderedQueryable<EventLog>>>()))
//            .ReturnsAsync(eventLog);

//        _customerTenantCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTenant, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTenant>, System.Linq.IOrderedQueryable<CustomerTenant>>>()))
//            .ReturnsAsync(customerTenant);

//        _customerPointLevelCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerPointLevel, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerPointLevel>, System.Linq.IOrderedQueryable<CustomerPointLevel>>>()))
//            .ReturnsAsync(existingLevel); // Customer already has this level

//        _advancedFormulaEvaluatorMock.Setup(x => x.EvaluateValueAsync(
//                It.IsAny<string>(),
//                It.IsAny<PromotionProcessingRequest>(),
//                It.IsAny<CancellationToken>()))
//            .ReturnsAsync((object?)0);

//        var service = CreateService();

//        // Act
//        await service.DoActionAsync(request, action, CancellationToken.None);

//        // Assert
//        _customerPointLevelCmdRepoMock.Verify(x => x.Add(It.IsAny<CustomerPointLevel>()), Times.Never);
//    }

//    #endregion

//    #region DebitPoint Tests with AllowNegativeBalance

//    [Fact]
//    public async Task DebitPoint_WhenAllowNegativeBalanceIsFalse_AndBalanceWouldGoNegative_ShouldReject()
//    {
//        // Arrange
//        var eventType = new EventType
//        {
//            Id = 1,
//            AllowNegativeBalance = false // Negative balance not allowed
//        };

//        var eventLog = new EventLog
//        {
//            Id = 1,
//            TenantId = 1,
//            EventTypeId = 1,
//            EventType = eventType
//        };

//        var point = new Point { Id = 1, Title = "Test Point", PointType = PointType.Normal };
//        var action = new PromotionAction
//        {
//            Id = 1,
//            ActionKind = PromotionActionKind.DebitPoint,
//            PointId = 1,
//            Point = point,
//            AmountFormula = "100"
//        };

//        var customerTenant = new CustomerTenant { Id = 1, CustomerId = 1, TenantId = 1 };
//        var lastTransaction = new CustomerTransaction
//        {
//            Id = 1,
//            Balance = 50, // Current balance is 50
//            PointId = 1,
//            Point = point
//        };

//        var request = new PromotionProcessingRequest(1, 1, 0, 0, new Customer { Id = 1 }, null);

//        _customerTenantCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTenant, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTenant>, System.Linq.IOrderedQueryable<CustomerTenant>>>()))
//            .ReturnsAsync(customerTenant);

//        _customerTransactionCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTransaction, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTransaction>, System.Linq.IOrderedQueryable<CustomerTransaction>>>()))
//            .ReturnsAsync(lastTransaction);

//        _eventLogQueryRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<EventLog, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<EventLog>, System.Linq.IOrderedQueryable<EventLog>>>()))
//            .ReturnsAsync(eventLog);

//        _advancedFormulaEvaluatorMock.Setup(x => x.EvaluateValueAsync(
//                It.IsAny<string>(),
//                It.IsAny<PromotionProcessingRequest>(),
//                It.IsAny<CancellationToken>()))
//            .ReturnsAsync((object?)100L);

//        var service = CreateService();

//        // Act
//        await service.DoActionAsync(request, action, CancellationToken.None);

//        // Assert - Should reject (not create transaction)
//        _customerTransactionCmdRepoMock.Verify(x => x.Add(It.IsAny<CustomerTransaction>()), Times.Never);
//    }

//    [Fact]
//    public async Task DebitPoint_WhenAllowNegativeBalanceIsTrue_AndBalanceWouldGoNegative_ShouldAllow()
//    {
//        // Arrange
//        var eventType = new EventType
//        {
//            Id = 1,
//            AllowNegativeBalance = true // Negative balance allowed
//        };

//        var eventLog = new EventLog
//        {
//            Id = 1,
//            TenantId = 1,
//            EventTypeId = 1,
//            EventType = eventType
//        };

//        var point = new Point { Id = 1, Title = "Test Point", PointType = PointType.Normal };
//        var action = new PromotionAction
//        {
//            Id = 1,
//            ActionKind = PromotionActionKind.DebitPoint,
//            PointId = 1,
//            Point = point,
//            AmountFormula = "100"
//        };

//        var customerTenant = new CustomerTenant { Id = 1, CustomerId = 1, TenantId = 1 };
//        var lastTransaction = new CustomerTransaction
//        {
//            Id = 1,
//            Balance = 50, // Current balance is 50, will go to -50
//            PointId = 1,
//            Point = point
//        };

//        var request = new PromotionProcessingRequest(1, 1, 0, 0, new Customer { Id = 1 }, null);

//        _customerTenantCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTenant, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTenant>, System.Linq.IOrderedQueryable<CustomerTenant>>>()))
//            .ReturnsAsync(customerTenant);

//        _customerTransactionCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTransaction, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTransaction>, System.Linq.IOrderedQueryable<CustomerTransaction>>>()))
//            .ReturnsAsync(lastTransaction);

//        _eventLogQueryRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<EventLog, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<EventLog>, System.Linq.IOrderedQueryable<EventLog>>>()))
//            .ReturnsAsync(eventLog);

//        var unitOfWorkMock = new Mock<IUnitOfWork>();
//        _customerTransactionCmdRepoMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

//        _advancedFormulaEvaluatorMock.Setup(x => x.EvaluateValueAsync(
//                It.IsAny<string>(),
//                It.IsAny<PromotionProcessingRequest>(),
//                It.IsAny<CancellationToken>()))
//            .ReturnsAsync((object?)100L);

//        var service = CreateService();

//        // Act
//        await service.DoActionAsync(request, action, CancellationToken.None);

//        // Assert - Should allow and create transaction
//        _customerTransactionCmdRepoMock.Verify(x => x.Add(It.Is<CustomerTransaction>(ct => 
//            ct.Debit == 100 && 
//            ct.TransactionType == CustomerTransactionType.Debit)), Times.Once);
//    }

//    [Fact]
//    public async Task DebitPoint_WhenAllowNegativeBalanceIsFalse_AndBalanceStaysPositive_ShouldAllow()
//    {
//        // Arrange
//        var eventType = new EventType
//        {
//            Id = 1,
//            AllowNegativeBalance = false // Negative balance not allowed
//        };

//        var eventLog = new EventLog
//        {
//            Id = 1,
//            TenantId = 1,
//            EventTypeId = 1,
//            EventType = eventType
//        };

//        var point = new Point { Id = 1, Title = "Test Point", PointType = PointType.Normal };
//        var action = new PromotionAction
//        {
//            Id = 1,
//            ActionKind = PromotionActionKind.DebitPoint,
//            PointId = 1,
//            Point = point,
//            AmountFormula = "30"
//        };

//        var customerTenant = new CustomerTenant { Id = 1, CustomerId = 1, TenantId = 1 };
//        var lastTransaction = new CustomerTransaction
//        {
//            Id = 1,
//            Balance = 100, // Current balance is 100, will go to 70 (still positive)
//            PointId = 1,
//            Point = point
//        };

//        var request = new PromotionProcessingRequest(1, 1, 0, 0, new Customer { Id = 1 }, null);

//        _customerTenantCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTenant, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTenant>, System.Linq.IOrderedQueryable<CustomerTenant>>>()))
//            .ReturnsAsync(customerTenant);

//        _customerTransactionCmdRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<CustomerTransaction, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<CustomerTransaction>, System.Linq.IOrderedQueryable<CustomerTransaction>>>()))
//            .ReturnsAsync(lastTransaction);

//        _eventLogQueryRepoMock.Setup(x => x.FirstOrDefaultAsync(
//                It.IsAny<System.Linq.Expressions.Expression<Func<EventLog, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<Func<System.Linq.IQueryable<EventLog>, System.Linq.IOrderedQueryable<EventLog>>>()))
//            .ReturnsAsync(eventLog);

//        var unitOfWorkMock = new Mock<IUnitOfWork>();
//        _customerTransactionCmdRepoMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

//        _advancedFormulaEvaluatorMock.Setup(x => x.EvaluateValueAsync(
//                It.IsAny<string>(),
//                It.IsAny<PromotionProcessingRequest>(),
//                It.IsAny<CancellationToken>()))
//            .ReturnsAsync((object?)30L);

//        var service = CreateService();

//        // Act
//        await service.DoActionAsync(request, action, CancellationToken.None);

//        // Assert - Should allow and create transaction
//        _customerTransactionCmdRepoMock.Verify(x => x.Add(It.Is<CustomerTransaction>(ct => 
//            ct.Debit == 30 && 
//            ct.TransactionType == CustomerTransactionType.Debit)), Times.Once);
//    }

//    #endregion

//    private PromotionActionService CreateService()
//    {
//        // Create mocks for all required dependencies
//        var rewardAssetServiceMock = new Mock<IRewardAssetInternalService>();
//        var smsServiceMock = new Mock<ISmsService>();
//        var evaluateFormulaServiceMock = new Mock<IEvaluateFormulaService>();
//        var customerParameterValueCmdRepoMock = new Mock<ICommandRepository<TenantAttributeValue, int>>();
//        var customerSegmentMembershipCmdRepoMock = new Mock<ICommandRepository<CustomerSegmentMembership, int>>();
//        var customerReferrerCmdRepoMock = new Mock<ICommandRepository<CustomerReferrer, int>>();
//        var referrerCodeRepoMock = new Mock<IQueryRepository<ReferrerCode, int>>();
//        var customerPlanQueryRepoMock = new Mock<IQueryRepository<CustomerPlan, int>>();
//        var externalApiServiceMock = new Mock<IExternalApiService>();
//        var lotteryRepoMock = new Mock<IQueryRepository<Hyper.Domain.Entities.Lotteries.Lottery, int>>();
//        var lotteryParticipantCmdRepoMock = new Mock<ICommandRepository<Hyper.Domain.Entities.Lotteries.LotteryParticipant, int>>();

//        return new PromotionActionService(
//            rewardAssetServiceMock.Object,
//            _pointBudgetServiceMock.Object,
//            smsServiceMock.Object,
//            evaluateFormulaServiceMock.Object,
//            _pointLevelServiceMock.Object,
//            _customerTransactionCmdRepoMock.Object,
//            customerParameterValueCmdRepoMock.Object,
//            customerSegmentMembershipCmdRepoMock.Object,
//            customerReferrerCmdRepoMock.Object,
//            _customerPointLevelCmdRepoMock.Object,
//            referrerCodeRepoMock.Object,
//            _customerTenantCmdRepoMock.Object,
//            _eventLogQueryRepoMock.Object,
//            customerPlanQueryRepoMock.Object,
//            externalApiServiceMock.Object,
//            lotteryRepoMock.Object,
//            lotteryParticipantCmdRepoMock.Object,
//            _loggerMock.Object,
//            null
//        );
//    }
//}

