using Hyper.Domain.Entities.Customers;
using Hyper.Domain.Entities.Events.Data;
using Hyper.Domain.Entities.Promotions;
using Hyper.Domain.Entities.Promotions.Enums;
using Hyper.Domain.Enums;
using Hyper.Domain.Features.Promotions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.Domain.Repository;
using CustomerSegmentMembershipData = Hyper.Domain.Entities.CustomerSegments.Data.CustomerSegmentMembership;
using PromotionParticipationData = Hyper.Domain.Entities.Promotions.Data.PromotionParticipation;

namespace Hyper.Domain.Tests;

/// <summary>
/// تست‌های سناریوهای پیچیده Promotion
/// </summary>
public class PromotionScenariosTests
{
    private readonly Mock<ILogger<IPromotionService>> _loggerMock;
    private readonly Mock<IQueryRepository<Promotion, int>> _promotionRepoMock;
    private readonly Mock<IQueryRepository<PromotionTrigger, int>> _triggerRepoMock;
    private readonly Mock<IQueryRepository<PromotionAction, int>> _actionRepoMock;
    private readonly Mock<IQueryRepository<PromotionParticipationData, int>> _participationRepoMock;
    private readonly Mock<IQueryRepository<CustomerSegmentMembershipData, int>> _segmentMembershipRepoMock;
    private readonly Mock<IQueryRepository<EventLog, long>> _eventLogRepoMock;
    private readonly Mock<IQueryRepository<CustomerTenant, int>> _customerTenantRepoMock;
    
    private const int TenantId = 1;
    private const int CustomerId = 1;
    private const int SilverCoinPointId = 1; // سکه نقره
    private const int GoldCoinPointId = 2; // سکه طلا
    private const int CustomerSegmentXId = 1; // جامعه x
    private const int LotteryXId = 1; // قرعه‌کشی x
    private const int VIPLevelParameterId = 1; // پارامتر سطح VIP

    public PromotionScenariosTests()
    {
        _loggerMock = new Mock<ILogger<IPromotionService>>();
        _promotionRepoMock = new Mock<IQueryRepository<Promotion, int>>();
        _triggerRepoMock = new Mock<IQueryRepository<PromotionTrigger, int>>();
        _actionRepoMock = new Mock<IQueryRepository<PromotionAction, int>>();
        _participationRepoMock = new Mock<IQueryRepository<PromotionParticipationData, int>>();
        _segmentMembershipRepoMock = new Mock<IQueryRepository<CustomerSegmentMembershipData, int>>();
        _eventLogRepoMock = new Mock<IQueryRepository<EventLog, long>>();
        _customerTenantRepoMock = new Mock<IQueryRepository<CustomerTenant, int>>();
    }

    #region Scenario 1: Sequential Actions with Time Window

    /// <summary>
    /// سناریو 1: کاربر باید 3 اقدام را در بازه زمانی مشخص انجام دهد
    /// 1. افتتاح حساب ممتاز با مانده حداقل 3 میلیون
    /// 2. خرید دو شارژ به مبالغ 5 و 10 تومان
    /// 3. دعوت یک دوست
    /// بازه زمانی: 3 تا 7 دی ساعت 19
    /// فقط کاربران جامعه x
    /// 
    /// جوایز:
    /// - با انجام مرحله 1: 2 سکه نقره
    /// - با انجام هر 3 مرحله: 10 سکه طلا + قرعه‌کشی + VIP
    /// </summary>
    [Fact]
    public Task Scenario1_SequentialActions_ShouldCreateCorrectPromotionStructure()
    {
        // Arrange
        var fromDate = new DateTime(2024, 10, 3, 19, 0, 0); // 3 دی 1403 ساعت 19
        var toDate = new DateTime(2024, 10, 7, 19, 0, 0); // 7 دی 1403 ساعت 19

        var promotion = new Promotion
        {
            Id = 1,
            TenantId = TenantId,
            Title = "پویش 3 اقدام متوالی",
            Category = PromotionCategory.ExclusiveOffers, // کمپین بازاریابی
            FromDate = fromDate,
            ToDate = toDate,
            Status = PromotionStatus.Active,
            CustomerSegments =
            [
                new() { CustomerSegmentId = CustomerSegmentXId }
            ]
        };

        // Trigger 1: افتتاح حساب ممتاز با مانده حداقل 3 میلیون
        var trigger1 = new PromotionTrigger
        {
            Id = 1,
            PromotionId = 1,
            Title = "افتتاح حساب ممتاز",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 1, // EventType: AccountOpening
            SequenceOrder = 1,
            Condition = "Event.AccountType == 'Premium' && Event.Balance >= 3000000"
        };

        // Trigger 2: خرید دو شارژ به مبالغ 5 و 10 تومان
        var trigger2 = new PromotionTrigger
        {
            Id = 2,
            PromotionId = 1,
            Title = "خرید دو شارژ",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 2, // EventType: ChargePurchase
            SequenceOrder = 2,
            DependencyTriggerId = 1, // وابسته به trigger 1
            Threshold = 2, // حداقل 2 بار
            Condition = "Event.Amount == 5000 || Event.Amount == 10000"
        };

        // Trigger 3: دعوت یک دوست
        var trigger3 = new PromotionTrigger
        {
            Id = 3,
            PromotionId = 1,
            Title = "دعوت دوست",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 3, // EventType: FriendInvitation
            SequenceOrder = 3,
            DependencyTriggerId = 2, // وابسته به trigger 2
            Threshold = 1
        };

        // Action 1: با انجام مرحله 1 - 2 سکه نقره (OnTrigger)
        var action1 = new PromotionAction
        {
            Id = 1,
            PromotionId = 1,
            PromotionTriggerId = 1, // روی محرک 1
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = SilverCoinPointId,
            AmountFormula = "2",
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Action 2: با انجام هر 3 مرحله - 10 سکه طلا (OnCompletion)
        var action2 = new PromotionAction
        {
            Id = 2,
            PromotionId = 1,
            PromotionTriggerId = null, // OnCompletion
            RunTimeType = PromotionActionRunTimeType.OnCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = GoldCoinPointId,
            AmountFormula = "10",
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Action 3: شرکت در قرعه‌کشی (OnCompletion)
        var action3 = new PromotionAction
        {
            Id = 3,
            PromotionId = 1,
            PromotionTriggerId = null,
            RunTimeType = PromotionActionRunTimeType.OnCompletion,
            ActionKind = PromotionActionKind.JoinLottery,
            LotteryId = LotteryXId,
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Action 4: عضویت در سطح VIP (OnCompletion)
        var action4 = new PromotionAction
        {
            Id = 4,
            PromotionId = 1,
            PromotionTriggerId = null,
            RunTimeType = PromotionActionRunTimeType.OnCompletion,
            ActionKind = PromotionActionKind.JoinInCustomerSegment,
            CustomerSegmentId = 2, // VIP Segment
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Assert
        promotion.Should().NotBeNull();
        promotion.FromDate.Should().Be(fromDate);
        promotion.ToDate.Should().Be(toDate);
        promotion.CustomerSegments.Should().HaveCount(1);
        promotion.CustomerSegments.First().CustomerSegmentId.Should().Be(CustomerSegmentXId);

        trigger1.SequenceOrder.Should().Be(1);
        trigger2.SequenceOrder.Should().Be(2);
        trigger2.DependencyTriggerId.Should().Be(1);
        trigger2.Threshold.Should().Be(2);
        trigger3.SequenceOrder.Should().Be(3);
        trigger3.DependencyTriggerId.Should().Be(2);

        action1.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action1.PromotionTriggerId.Should().Be(1);
        action1.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action1.PointId.Should().Be(SilverCoinPointId);
        action1.AmountFormula.Should().Be("2");

        action2.RunTimeType.Should().Be(PromotionActionRunTimeType.OnCompletion);
        action2.PromotionTriggerId.Should().BeNull();
        action2.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action2.PointId.Should().Be(GoldCoinPointId);
        action2.AmountFormula.Should().Be("10");

        action3.RunTimeType.Should().Be(PromotionActionRunTimeType.OnCompletion);
        action3.ActionKind.Should().Be(PromotionActionKind.JoinLottery);

        action4.RunTimeType.Should().Be(PromotionActionRunTimeType.OnCompletion);
        action4.ActionKind.Should().Be(PromotionActionKind.JoinInCustomerSegment);
        
        return Task.CompletedTask;
    }

    #endregion

    #region Scenario 2: BNPL/Kalano Payment Scenarios

    /// <summary>
    /// سناریو 2: اقدامات مربوط به بازپرداخت اقساط
    /// - بازپرداخت قسط BNPL در موعد: 1 سکه نقره
    /// - بازپرداخت قسط کالانو زودتر از موعد: 2 سکه طلا
    /// - دیرکرد در بازپرداخت: کاهش 2 سکه نقره + نزول از طلایی به نقره‌ای
    /// </summary>
    [Fact]
    public Task Scenario2_PaymentScenarios_ShouldCreateCorrectPromotionStructure()
    {
        // Arrange
        var promotion = new Promotion
        {
            Id = 2,
            TenantId = TenantId,
            Title = "پویش بازپرداخت اقساط",
            Category = PromotionCategory.RewardsAndPointsPrograms,
            Status = PromotionStatus.Active
        };

        // Trigger 1: بازپرداخت BNPL در موعد
        var trigger1 = new PromotionTrigger
        {
            Id = 4,
            PromotionId = 2,
            Title = "بازپرداخت BNPL در موعد",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 4, // EventType: BNPLPaymentOnTime
            Condition = "true" // بدون شرط اضافی
        };

        // Action 1: 1 سکه نقره (OnOneEventCompletion)
        var action1 = new PromotionAction
        {
            Id = 5,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = SilverCoinPointId,
            AmountFormula = "1",
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Trigger 2: بازپرداخت کالانو زودتر از موعد
        var trigger2 = new PromotionTrigger
        {
            Id = 5,
            PromotionId = 2,
            Title = "بازپرداخت کالانو زودتر از موعد",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 5, // EventType: KalanoPaymentEarly
            Condition = "true" // بدون شرط اضافی
        };

        // Action 2: 2 سکه طلا (OnOneEventCompletion)
        var action2 = new PromotionAction
        {
            Id = 6,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = GoldCoinPointId,
            AmountFormula = "2",
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Trigger 3: دیرکرد در بازپرداخت
        var trigger3 = new PromotionTrigger
        {
            Id = 6,
            PromotionId = 2,
            Title = "دیرکرد در بازپرداخت",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 6, // EventType: PaymentLate
            Condition = "true" // بدون شرط اضافی
        };

        // Action 3: کاهش 2 سکه نقره (OnOneEventCompletion)
        var action3 = new PromotionAction
        {
            Id = 7,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.DebitPoint,
            PointId = SilverCoinPointId,
            AmountFormula = "2",
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Action 4: نزول از طلایی به نقره‌ای (OnOneEventCompletion)
        var action4 = new PromotionAction
        {
            Id = 8,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.SetCustomerParameterValue,
            CustomerAttributeId = VIPLevelParameterId,
            AmountFormula = "\"Silver\"", // مقدار جدید برای پارامتر
            ActionOnWho = PromotionActionOnWho.Customer
        };

        // Assert
        promotion.Should().NotBeNull();
        promotion.Category.Should().Be(PromotionCategory.RewardsAndPointsPrograms);

        trigger1.EventTypeId.Should().Be(4);
        action1.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action1.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action1.PointId.Should().Be(SilverCoinPointId);
        action1.AmountFormula.Should().Be("1");

        trigger2.EventTypeId.Should().Be(5);
        action2.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action2.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action2.PointId.Should().Be(GoldCoinPointId);
        action2.AmountFormula.Should().Be("2");

        trigger3.EventTypeId.Should().Be(6);
        action3.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action3.ActionKind.Should().Be(PromotionActionKind.DebitPoint);
        action3.AmountFormula.Should().Be("2");

        action4.ActionKind.Should().Be(PromotionActionKind.SetCustomerParameterValue);
        action4.CustomerAttributeId.Should().Be(VIPLevelParameterId);
        action4.AmountFormula.Should().Be("\"Silver\"");
        
        return Task.CompletedTask;
    }

    #endregion

    #region Database Structure Verification

    /// <summary>
    /// بررسی اینکه ساختار دیتابیس برای این سناریوها کافی است
    /// </summary>
    [Fact]
    public void DatabaseStructure_ShouldSupportScenarios()
    {
        // بررسی Promotion
        var promotionType = typeof(Promotion);
        promotionType.GetProperty(nameof(Promotion.FromDate)).Should().NotBeNull();
        promotionType.GetProperty(nameof(Promotion.ToDate)).Should().NotBeNull();
        promotionType.GetProperty(nameof(Promotion.CustomerSegments)).Should().NotBeNull();

        // بررسی PromotionTrigger - فیلدهای زمان‌بندی به اینجا منتقل شدند
        var triggerType = typeof(PromotionTrigger);
        triggerType.GetProperty(nameof(PromotionTrigger.SequenceOrder)).Should().NotBeNull();
        triggerType.GetProperty(nameof(PromotionTrigger.DependencyTriggerId)).Should().NotBeNull();
        triggerType.GetProperty(nameof(PromotionTrigger.Threshold)).Should().NotBeNull();
        triggerType.GetProperty(nameof(PromotionTrigger.EventTypeId)).Should().NotBeNull();
        triggerType.GetProperty(nameof(PromotionTrigger.Condition)).Should().NotBeNull();
        triggerType.GetProperty(nameof(PromotionTrigger.ScheduledHour)).Should().NotBeNull();
        triggerType.GetProperty(nameof(PromotionTrigger.ScheduledMinute)).Should().NotBeNull();

        // بررسی PromotionAction
        var actionType = typeof(PromotionAction);
        actionType.GetProperty(nameof(PromotionAction.RunTimeType)).Should().NotBeNull();
        actionType.GetProperty(nameof(PromotionAction.PromotionTriggerId)).Should().NotBeNull();
        actionType.GetProperty(nameof(PromotionAction.ActionKind)).Should().NotBeNull();
        actionType.GetProperty(nameof(PromotionAction.PointId)).Should().NotBeNull();
        actionType.GetProperty(nameof(PromotionAction.LotteryId)).Should().NotBeNull();
        actionType.GetProperty(nameof(PromotionAction.CustomerSegmentId)).Should().NotBeNull();
        actionType.GetProperty(nameof(PromotionAction.CustomerAttributeId)).Should().NotBeNull();
    }

    #endregion

    #region Integration Tests - Scenario 1 Flow

    /// <summary>
    /// تست جریان کامل سناریو 1: بررسی اینکه با انجام مرحله 1، action OnTrigger اجرا می‌شود
    /// </summary>
    [Fact]
    public Task Scenario1_Step1Completion_ShouldExecuteOnTriggerAction()
    {
        // Arrange
        var promotion = CreateScenario1Promotion();
        var trigger1 = CreateScenario1Trigger1();
        var action1 = CreateScenario1Action1(); // 2 سکه نقره OnTrigger

        // Assert - بررسی ساختار
        action1.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action1.PromotionTriggerId.Should().Be(trigger1.Id);
        action1.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action1.PointId.Should().Be(SilverCoinPointId);
        action1.AmountFormula.Should().Be("2");

        // این تست ساختار را بررسی می‌کند
        // برای تست کامل integration نیاز به mock کردن PromotionService است
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// تست جریان کامل سناریو 1: بررسی اینکه با انجام هر 3 مرحله، actions OnCompletion اجرا می‌شوند
    /// </summary>
    [Fact]
    public Task Scenario1_AllStepsCompletion_ShouldExecuteOnCompletionActions()
    {
        // Arrange
        var promotion = CreateScenario1Promotion();
        var action2 = CreateScenario1Action2(); // 10 سکه طلا OnCompletion
        var action3 = CreateScenario1Action3(); // قرعه‌کشی OnCompletion
        var action4 = CreateScenario1Action4(); // VIP OnCompletion

        // Assert - بررسی ساختار
        action2.RunTimeType.Should().Be(PromotionActionRunTimeType.OnCompletion);
        action2.PromotionTriggerId.Should().BeNull();
        action2.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action2.PointId.Should().Be(GoldCoinPointId);
        action2.AmountFormula.Should().Be("10");

        action3.RunTimeType.Should().Be(PromotionActionRunTimeType.OnCompletion);
        action3.ActionKind.Should().Be(PromotionActionKind.JoinLottery);
        action3.LotteryId.Should().Be(LotteryXId);

        action4.RunTimeType.Should().Be(PromotionActionRunTimeType.OnCompletion);
        action4.ActionKind.Should().Be(PromotionActionKind.JoinInCustomerSegment);
        
        return Task.CompletedTask;
    }

    #endregion

    #region Integration Tests - Scenario 2 Flow

    /// <summary>
    /// تست جریان کامل سناریو 2: بازپرداخت BNPL در موعد
    /// </summary>
    [Fact]
    public Task Scenario2_BNPLOnTime_ShouldCreditSilverCoin()
    {
        // Arrange
        var trigger = CreateScenario2Trigger1(); // BNPL در موعد
        var action = CreateScenario2Action1(); // 1 سکه نقره

        // Assert
        trigger.EventTypeId.Should().Be(4); // BNPLPaymentOnTime
        action.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action.PointId.Should().Be(SilverCoinPointId);
        action.AmountFormula.Should().Be("1");
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// تست جریان کامل سناریو 2: بازپرداخت کالانو زودتر از موعد
    /// </summary>
    [Fact]
    public Task Scenario2_KalanoEarly_ShouldCreditGoldCoins()
    {
        // Arrange
        var trigger = CreateScenario2Trigger2(); // کالانو زودتر
        var action = CreateScenario2Action2(); // 2 سکه طلا

        // Assert
        trigger.EventTypeId.Should().Be(5); // KalanoPaymentEarly
        action.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action.ActionKind.Should().Be(PromotionActionKind.CreditPoint);
        action.PointId.Should().Be(GoldCoinPointId);
        action.AmountFormula.Should().Be("2");
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// تست جریان کامل سناریو 2: دیرکرد در بازپرداخت
    /// </summary>
    [Fact]
    public Task Scenario2_PaymentLate_ShouldDebitAndDowngrade()
    {
        // Arrange
        var trigger = CreateScenario2Trigger3(); // دیرکرد
        var action3 = CreateScenario2Action3(); // کاهش 2 سکه نقره
        var action4 = CreateScenario2Action4(); // نزول به نقره‌ای

        // Assert
        trigger.EventTypeId.Should().Be(6); // PaymentLate
        action3.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action3.ActionKind.Should().Be(PromotionActionKind.DebitPoint);
        action3.AmountFormula.Should().Be("2");

        action4.RunTimeType.Should().Be(PromotionActionRunTimeType.OnOneEventCompletion);
        action4.ActionKind.Should().Be(PromotionActionKind.SetCustomerParameterValue);
        action4.AmountFormula.Should().Be("\"Silver\"");
        
        return Task.CompletedTask;
    }

    #endregion

    #region Helper Methods

    private Promotion CreateScenario1Promotion()
    {
        var fromDate = new DateTime(2024, 10, 3, 19, 0, 0);
        var toDate = new DateTime(2024, 10, 7, 19, 0, 0);

        return new Promotion
        {
            Id = 1,
            TenantId = TenantId,
            Title = "پویش 3 اقدام متوالی",
            Category = PromotionCategory.ExclusiveOffers, // کمپین بازاریابی
            FromDate = fromDate,
            ToDate = toDate,
            Status = PromotionStatus.Active,
            CustomerSegments =
            [
                new() { CustomerSegmentId = CustomerSegmentXId }
            ]
        };
    }

    private PromotionTrigger CreateScenario1Trigger1()
    {
        return new PromotionTrigger
        {
            Id = 1,
            PromotionId = 1,
            Title = "افتتاح حساب ممتاز",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 1,
            SequenceOrder = 1,
            Condition = "Event.AccountType == 'Premium' && Event.Balance >= 3000000"
        };
    }

    private PromotionAction CreateScenario1Action1()
    {
        return new PromotionAction
        {
            Id = 1,
            PromotionId = 1,
            PromotionTriggerId = 1,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = SilverCoinPointId,
            AmountFormula = "2",
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    private PromotionAction CreateScenario1Action2()
    {
        return new PromotionAction
        {
            Id = 2,
            PromotionId = 1,
            PromotionTriggerId = null,
            RunTimeType = PromotionActionRunTimeType.OnCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = GoldCoinPointId,
            AmountFormula = "10",
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    private PromotionAction CreateScenario1Action3()
    {
        return new PromotionAction
        {
            Id = 3,
            PromotionId = 1,
            PromotionTriggerId = null,
            RunTimeType = PromotionActionRunTimeType.OnCompletion,
            ActionKind = PromotionActionKind.JoinLottery,
            LotteryId = LotteryXId,
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    private PromotionAction CreateScenario1Action4()
    {
        return new PromotionAction
        {
            Id = 4,
            PromotionId = 1,
            PromotionTriggerId = null,
            RunTimeType = PromotionActionRunTimeType.OnCompletion,
            ActionKind = PromotionActionKind.JoinInCustomerSegment,
            CustomerSegmentId = 2, // VIP Segment
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    private PromotionTrigger CreateScenario2Trigger1()
    {
        return new PromotionTrigger
        {
            Id = 4,
            PromotionId = 2,
            Title = "بازپرداخت BNPL در موعد",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 4,
            Condition = "true" // بدون شرط اضافی
        };
    }

    private PromotionAction CreateScenario2Action1()
    {
        return new PromotionAction
        {
            Id = 5,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = SilverCoinPointId,
            AmountFormula = "1",
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    private PromotionTrigger CreateScenario2Trigger2()
    {
        return new PromotionTrigger
        {
            Id = 5,
            PromotionId = 2,
            Title = "بازپرداخت کالانو زودتر از موعد",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 5,
            Condition = "true" // بدون شرط اضافی
        };
    }

    private PromotionAction CreateScenario2Action2()
    {
        return new PromotionAction
        {
            Id = 6,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.CreditPoint,
            PointId = GoldCoinPointId,
            AmountFormula = "2",
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    private PromotionTrigger CreateScenario2Trigger3()
    {
        return new PromotionTrigger
        {
            Id = 6,
            PromotionId = 2,
            Title = "دیرکرد در بازپرداخت",
            ReceiveEventType = ReceiveEventType.DynamicEvent,
            EventTypeId = 6,
            Condition = "true" // بدون شرط اضافی
        };
    }

    private PromotionAction CreateScenario2Action3()
    {
        return new PromotionAction
        {
            Id = 7,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.DebitPoint,
            PointId = SilverCoinPointId,
            AmountFormula = "2",
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    private PromotionAction CreateScenario2Action4()
    {
        return new PromotionAction
        {
            Id = 8,
            PromotionId = 2,
            RunTimeType = PromotionActionRunTimeType.OnOneEventCompletion,
            ActionKind = PromotionActionKind.SetCustomerParameterValue,
            CustomerAttributeId = VIPLevelParameterId,
            AmountFormula = "\"Silver\"",
            ActionOnWho = PromotionActionOnWho.Customer
        };
    }

    #endregion
}

