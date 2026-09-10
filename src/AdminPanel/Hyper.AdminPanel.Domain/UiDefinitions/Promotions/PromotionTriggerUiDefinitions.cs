namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

/// <summary>
/// UI Definitions برای محرک پویش
/// 
/// ساختار گروه‌بندی:
/// - دسته 1: نوع رویداد و اطلاعات پایه
/// - دسته 2: شرط محرک
/// - دسته 3: پنجره تعدادی
/// - دسته 4: پنجره زمانی
/// - دسته 5: فلو و توالی
/// </summary>
public class PromotionTriggerUiDefinitions : CRUDDefinition<PromotionTrigger>
{
    public override string? Icon => "fa fa-bolt";

    protected override void IndexFormViewModel()
    {
        AddColumns(
              nameof(PromotionTrigger.Promotion)
            , nameof(PromotionTrigger.Title)
            , nameof(PromotionTrigger.ReceiveEventType)
            , nameof(PromotionTrigger.EventSource)
            , nameof(PromotionTrigger.Condition)
            , nameof(PromotionTrigger.TriggerGroup)
        );
        AddSubjectColumn<WindowDefinitions>();
        AddOrderBy(nameof(PromotionTrigger.SequenceOrder));
    }

    enum Groups
    {
        EventGroup,          // دسته 1: نوع رویداد
        CountWindow,         // دسته 3: پنجره تعدادی
        TimeWindow,          // دسته 4: پنجره زمانی
        ReviewTime,
        FlowSequence,         // دسته 5: فلو و توالی
        // گروه‌های فیلدهای تکمیلی منبع رویداد
        DynamicEventGroup,    // گروه رویداد پویا: Channel + EventType + ProductCategory + Product
        PointLevelGroup,      // گروه سطح امتیاز: PointLevel
        RewardGroup,          // گروه پاداش: RewardCategory + Reward
        SurveyGroup,          // گروه نظرسنجی: Survey
        LotteryGroup,         // گروه قرعه‌کشی: Lottery
        FeedbackGroup,        // گروه بازخورد: FeedbackType
        ForumGroup,           // گروه انجمن: ForumTopic
        PlanGroup,            // گروه طرح: Plan
        CallCenterGroup,      // گروه مرکز تماس: CallCenterInteractionType
        PointGroup,           // گروه مربوط به امتیاز TransferPoint و ChangePoint
    }

    protected override void CUDFormsViewModel()
    {
        // ===== دسته 1: نوع رویداد و اطلاعات پایه =====
        AddGroup(nameof(Groups.EventGroup), "نوع رویداد و اطلاعات پایه");
        {
            AddFields(
                  nameof(PromotionTrigger.Promotion)
                , nameof(PromotionTrigger.Title)
                , nameof(PromotionTrigger.ReceiveEventType)
                , nameof(PromotionTrigger.TriggerGroup)
                , nameof(PromotionTrigger.SequenceOrder)
                );
            // استفاده از MultilineTextInput برای شرط (می‌تواند طولانی باشد)
            AddField(nameof(PromotionTrigger.Condition), eControlTypeId.MultilineTextInput);
            EndGroup();
        }

        if (IsCreateForm)
        {
            return;
        }
        // ===== دسته 2: گروه‌های فیلدهای تکمیلی منبع رویداد =====

        // گروه رویداد پویا: Channel + EventType + ProductCategory + Product
        AddGroup(nameof(Groups.DynamicEventGroup), "رویداد پویا");
        {
            AddFields(
                  nameof(PromotionTrigger.Channel)
                , nameof(PromotionTrigger.EventType)
                , nameof(PromotionTrigger.ProductCategory)
                , nameof(PromotionTrigger.Product)
                );
            EndGroup();
        }

        // گروه سطح امتیاز: PointLevel
        AddGroup(nameof(Groups.PointLevelGroup), "سطح امتیاز");
        {
            AddField(nameof(PromotionTrigger.PointLevel));
            EndGroup();
        }

        AddGroup(nameof(Groups.PointGroup), "امتیاز");
        {
            AddFields(nameof(PromotionTrigger.FromPoint)
                         , nameof(PromotionTrigger.ToPoint));
            EndGroup();
        }

        // گروه پاداش: RewardCategory + Reward
        AddGroup(nameof(Groups.RewardGroup), "پاداش");
        {
            AddFields(
                nameof(PromotionTrigger.RewardCategory),
                nameof(PromotionTrigger.Reward)
            );
            EndGroup();
        }

        // گروه نظرسنجی: Survey
        AddGroup(nameof(Groups.SurveyGroup), "نظرسنجی");
        {
            AddField(nameof(PromotionTrigger.Survey));
            EndGroup();
        }

        // گروه قرعه‌کشی: Lottery
        AddGroup(nameof(Groups.LotteryGroup), "قرعه‌کشی");
        {
            AddField(nameof(PromotionTrigger.Lottery));
            EndGroup();
        }

        // گروه بازخورد: FeedbackType
        AddGroup(nameof(Groups.FeedbackGroup), "بازخورد");
        {
            AddField(nameof(PromotionTrigger.FeedbackType));
            EndGroup();
        }

        // گروه انجمن: ForumTopic
        AddGroup(nameof(Groups.ForumGroup), "انجمن");
        {
            AddField(nameof(PromotionTrigger.ForumTopic));
            EndGroup();
        }

        // گروه طرح: Plan
        AddGroup(nameof(Groups.PlanGroup), "طرح اشتراک");
        {
            AddField(nameof(PromotionTrigger.Plan));
            EndGroup();
        }

        // گروه مرکز تماس: CallCenterInteractionType
        AddGroup(nameof(Groups.CallCenterGroup), "مرکز تماس");
        {
            AddField(nameof(PromotionTrigger.CallCenterInteractionType));
            EndGroup();
        }

        AddHiddenField(nameof(PromotionTrigger.TenantId));
    }

    protected override void UIRules()
    {
        base.UIRules(form);
        if (IsCreateForm)
        {
            return;
        }

        const string typeField = nameof(PromotionTrigger.ReceiveEventType);

        // ===== قوانین نمایش گروه‌های فیلدهای تکمیلی بر اساس نوع رویداد =====
        // استفاده از گروه‌ها به جای فیلدهای جداگانه برای بهبود عملکرد و خوانایی

        // DynamicEvent - رویداد پویا: گروه Channel + EventType + ProductCategory + Product
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.DynamicEvent}",
            nameof(Groups.DynamicEventGroup));

        // UpgradePointLevel - ارتقاء سطح امتیاز: گروه PointLevel
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.UpgradePointLevel}",
            nameof(Groups.PointLevelGroup));

        // PurchaseReward / ConsumeReward - خرید/مصرف پاداش: گروه Reward + RewardCategory
        ShowHide(typeField,
            $"(q[{typeField}]=={(int)ReceiveEventType.PurchaseReward}) || (q[{typeField}]=={(int)ReceiveEventType.ConsumeReward})",
            nameof(Groups.RewardGroup));

        // ParticipateInSurvey - شرکت در نظرسنجی: گروه Survey
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.ParticipateInSurvey}",
            nameof(Groups.SurveyGroup));

        // ParticipateInLottery - شرکت در قرعه‌کشی: گروه Lottery
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.ParticipateInLottery}",
            nameof(Groups.LotteryGroup));

        // InteractInFeedback - تعامل در بازخورد: گروه FeedbackType
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.InteractInFeedback}",
            nameof(Groups.FeedbackGroup));

        // InteractInForum - تعامل در انجمن: گروه ForumTopic
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.InteractInForum}",
            nameof(Groups.ForumGroup));

        // PurchasePlan - خرید طرح اشتراک: گروه Plan
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.PurchasePlan}",
            nameof(Groups.PlanGroup));

        // CustomerCallCenterInteraction - تعامل با مرکز تماس: گروه CallCenterInteractionType
        ShowHide(typeField,
            $"q[{typeField}]=={(int)ReceiveEventType.CustomerCallCenterInteraction}",
            nameof(Groups.CallCenterGroup));
        ShowHide(typeField,
            $"(q[{typeField}]=={(int)ReceiveEventType.TransferPoint}) || (q[{typeField}]=={(int)ReceiveEventType.ChangePoint})",
            nameof(Groups.PointGroup));

        SetTenantFilter(nameof(PromotionTrigger.Channel));
        SetTenantFilter(nameof(PromotionTrigger.EventType));
        SetTenantFilter(nameof(PromotionTrigger.Channel));
        SetTenantFilter(nameof(PromotionTrigger.ProductCategory));
        SetTenantFilter(nameof(PromotionTrigger.FromPoint));
        SetTenantFilter(nameof(PromotionTrigger.ToPoint));
        SetTenantFilter(nameof(PromotionTrigger.RewardCategory));
        SetTenantFilter(nameof(PromotionTrigger.ForumTopic));
        // پاداش بر اساس گروه پاداش انتخاب شده (سلسله‌مراتبی)
        FilterFormula(null,
            nameof(PromotionTrigger.PointLevel),
            $"({nameof(PointLevel.Point)}.{nameof(ISubOfTenant.TenantId)})==q[{nameof(PromotionTrigger.TenantId)}]");
        FilterFormula(nameof(PromotionTrigger.RewardCategory),
            nameof(PromotionTrigger.Reward), 
            $"{TenantFilter()} And (({nameof(Reward.RewardCategoryId)}==q[{nameof(PromotionTrigger.RewardCategory)}]) || (q[{nameof(PromotionTrigger.RewardCategory)}]==null))");
        // محصول بر اساس گروه محصول انتخاب شده (سلسله‌مراتبی)
        FilterFormula(nameof(PromotionTrigger.ProductCategory),
            nameof(PromotionTrigger.Product),
            $"{TenantFilter()} And (({nameof(Product.ProductCategoryId)}==q[{nameof(PromotionTrigger.ProductCategory)}]) || (q[{nameof(PromotionTrigger.ProductCategory)}]==null))");

        SetPromotionFilter(nameof(PromotionTrigger.Survey));
        SetPromotionFilter(nameof(PromotionTrigger.Lottery));
        SetPromotionFilter(nameof(PromotionTrigger.Plan));

        void SetPromotionFilter(string name)
        {
            FilterFormula(null, name, PromotionFilter());
        }
        static string PromotionFilter()
        {
            return $"{nameof(ISubOfPromotion.PromotionId)}==q[{nameof(PromotionTrigger.Promotion)}]";
        }
        void SetTenantFilter(string name)
        {
            FilterFormula(null, name, TenantFilter());
        }
        static string TenantFilter()
        {
            return $"{nameof(ISubOfTenant.TenantId)}==q[{nameof(PromotionTrigger.TenantId)}]";
        }
    }

    public class WindowDefinitions() : SubjectEditForm<WindowDefinitions>("تنظیمات پنجره")
    {
        protected override void ViewModel()
        {
            AddField(nameof(PromotionTrigger.Promotion), eControlPropertyId.ReadOnly);
            AddField(nameof(PromotionTrigger.ReceiveEventType), eControlPropertyId.ReadOnly);
            // ===== دسته 3: پنجره تعدادی =====
            AddGroup( nameof(Groups.CountWindow), "پنجره تعدادی");
            {
                AddFields(
                      nameof(PromotionTrigger.CounterWindowMode)
                    , nameof(PromotionTrigger.Threshold)
                    , nameof(PromotionTrigger.MaxThreshold)
                    );
                EndGroup();
            }

            // ===== دسته 4: پنجره زمانی =====
            AddGroup(nameof(Groups.TimeWindow), "بازه تاریخی مستقل");
            {
                AddFields(
                    nameof(PromotionTrigger.HasIndependentTimeWindow),
                    nameof(PromotionTrigger.FromDate),
                    nameof(PromotionTrigger.ToDate),
                    nameof(PromotionTrigger.FromHour),
                    nameof(PromotionTrigger.ToHour)
                );
                EndGroup();
            }

            AddGroup(nameof(Groups.ReviewTime), "زمان بررسی");
            {
                AddFields(
                      nameof(PromotionTrigger.CheckTimeKind)
                    , nameof(PromotionTrigger.ScheduledHour)
                    , nameof(PromotionTrigger.ScheduledMinute)
                    , nameof(PromotionTrigger.ScheduledMonth)
                    , nameof(PromotionTrigger.ScheduledWeekDay)
                    , nameof(PromotionTrigger.ScheduledMonthDay)
                    );
                EndGroup();
            }

            // ===== دسته 5: فلو و توالی =====
            AddGroup(nameof(Groups.FlowSequence), "فلو و توالی");
            {
                AddFields(
                      nameof(PromotionTrigger.SequenceOrder)
                    , nameof(PromotionTrigger.FlowType)
                    , nameof(PromotionTrigger.DependencyTrigger)
                    );
                EndGroup();
            }
            AddHiddenField(nameof(PromotionTrigger.TenantId));
        }

        protected override void UIRules()
        {
            base.UIRules();
            // ===== پنجره زمانی (دسته 4) =====
            const string hasTimeWindowField = nameof(PromotionTrigger.HasIndependentTimeWindow);

            // نمایش فیلدهای بازه زمانی فقط وقتی HasIndependentTimeWindow = true
            ShowHide(hasTimeWindowField,
                $"q[{hasTimeWindowField}]",
                nameof(PromotionTrigger.FromDate),
                nameof(PromotionTrigger.ToDate),
                nameof(PromotionTrigger.FromHour),
                nameof(PromotionTrigger.ToHour));

            // ===== فلو و توالی (دسته 5) =====
            const string checkTimeKindField = nameof(PromotionTrigger.CheckTimeKind);

            // نمایش ScheduledMonth فقط وقتی IsScheduled = true
            ShowHide(checkTimeKindField,
                $"q[{checkTimeKindField}]=={(int)SchedulingKind.Monthly}",
                nameof(PromotionTrigger.ScheduledMonth));

            // نمایش WeekDay فقط وقتی IsScheduled = true و CheckTimeKind = Weekly
            ShowHide(checkTimeKindField,
                $"q[{checkTimeKindField}]=={(int)PromotionTriggerCheckTimeKind.ScheduledInEndOfEachWeek}",
                nameof(PromotionTrigger.ScheduledWeekDay));

            // نمایش MonthDay فقط وقتی IsScheduled = true و SchedulingKind = Monthly یا Yearly
            ShowHide(checkTimeKindField,
                $"(q[{checkTimeKindField}]=={(int)SchedulingKind.Monthly} || q[{checkTimeKindField}]=={(int)SchedulingKind.Yearly})",
                nameof(PromotionTrigger.ScheduledMonthDay));

            SetPromotionFilter(nameof(PromotionTrigger.DependencyTrigger));

            void SetPromotionFilter(string name)
            {
                FilterFormula(null, name, PromotionFilter());
            }
            static string PromotionFilter()
            {
                return $"{nameof(ISubOfPromotion.PromotionId)}==q[{nameof(PromotionTrigger.Promotion)}]";
            }
        }
    }
}