namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public class PromotionActionUiDefinitions : CRUDDefinition<PromotionAction>
{
    public override string? Icon => "fa fa-bolt";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(PromotionAction.Promotion),
            nameof(PromotionAction.RunTimeType),
            nameof(PromotionAction.ActionKind),
            nameof(PromotionAction.PromotionTrigger),
            nameof(PromotionAction.NotificationSendMethod),
            nameof(PromotionAction.Point),
            nameof(PromotionAction.Reward),
            nameof(PromotionAction.Lottery),
            nameof(PromotionAction.CustomerAttribute),
            nameof(PromotionAction.CustomerSegment),
            nameof(PromotionAction.ExternalApi),
            nameof(PromotionAction.ActionOnWho)
        );
    }
    
    enum Groups
    {
        BaseSettings,          // دسته 1: اطلاعات پایه
        TargetSettings,        // دسته 2: هدف و اعطا
        AmountSettings,        // دسته 3: محاسبه مقدار
        ExternalApiSettings,   // دسته 4: فراخوانی API بیرونی
        NotificationSettings   // دسته 5: اعلان و پیام
    }

    protected override void CUDFormsViewModel()
    {
        // ===== دسته 1: اطلاعات پایه =====
        form.AddGroup(nameof(Groups.BaseSettings), "اطلاعات پایه");
        {
            AddFields(
                nameof(PromotionAction.Promotion),
                nameof(PromotionAction.RunTimeType),
                nameof(PromotionAction.PromotionTrigger),
                nameof(PromotionAction.ActionOnWho),
                nameof(PromotionAction.ActionKind),
                nameof(PromotionAction.Order),
                nameof(PromotionAction.NotificationSendMethod)
            );
            form.EndGroup();
        }

        // ===== دسته 2: هدف و اعطا =====
        form.AddGroup(nameof(Groups.TargetSettings), "هدف و اعطا");
        {
            AddFields(
                nameof(PromotionAction.Point),
                nameof(PromotionAction.PointLevel),
                nameof(PromotionAction.Reward),
                nameof(PromotionAction.Lottery),
                nameof(PromotionAction.CustomerAttribute),
                nameof(PromotionAction.CustomerSegment)
            );
            form.EndGroup();
        }

        // ===== دسته 3: محاسبه مقدار =====
        form.AddGroup(nameof(Groups.AmountSettings), "محاسبه مقدار");
        {
            AddField(nameof(PromotionAction.AmountFormula), eControlTypeId.MultilineTextInput);
            form.EndGroup();
        }

        // ===== دسته 4: فراخوانی API بیرونی =====
        form.AddGroup(nameof(Groups.ExternalApiSettings), "فراخوانی API بیرونی");
        {
            AddField(nameof(PromotionAction.ExternalApi));
            AddField(nameof(PromotionAction.PathParameterMappingsJson), eControlTypeId.MultilineTextInput);
            AddField(nameof(PromotionAction.QueryParameterMappingsJson), eControlTypeId.MultilineTextInput);
            AddField(nameof(PromotionAction.HeaderMappingsJson), eControlTypeId.MultilineTextInput);
            AddField(nameof(PromotionAction.BodyTemplate), eControlTypeId.MultilineTextInput);
            AddField(nameof(PromotionAction.BodyContentType));
            AddField(nameof(PromotionAction.ResponseMappingsJson), eControlTypeId.MultilineTextInput);
            form.EndGroup();
        }

        // ===== دسته 5: اعلان و پیام =====
        form.AddGroup(nameof(Groups.NotificationSettings), "اعلان و پیام");
        {
            AddField(nameof(PromotionAction.MessageTemplate), eControlTypeId.MultilineTextInput);
            form.EndGroup();
        }
    }
    
    protected override void UIRules(FormDefinition form)
    {
        /*const string runTimeTypeField = nameof(PromotionAction.RunTimeType);
        const string actionKindField = nameof(PromotionAction.ActionKind);

        // PromotionTriggerId فقط برای OnTrigger
        string onTriggerAction = $"q[{runTimeTypeField}]=={(int)PromotionActionRunTimeType.OnOneEventCompletion}";
        form.ShowHide(runTimeTypeField, onTriggerAction, nameof(PromotionAction.PromotionTrigger));

        string pointActionsCondition =
            $"(q[{actionKindField}]=={(int)PromotionActionKind.CreditPoint}) || " +
            $"(q[{actionKindField}]=={(int)PromotionActionKind.DebitPoint}) || " +
            $"(q[{actionKindField}]=={(int)PromotionActionKind.SetPointBalance})";

        form.ShowHide(actionKindField, pointActionsCondition, nameof(PromotionAction.Point));

        string pointLevelCondition =
            $"q[{actionKindField}]=={(int)PromotionActionKind.SetPointLevel}";
        form.ShowHide(actionKindField, pointLevelCondition, nameof(PromotionAction.PointLevel));

        string customerParameterCondition =
            $"q[{actionKindField}]=={(int)PromotionActionKind.SetCustomerParameterValue}";
        form.ShowHide(actionKindField, customerParameterCondition, nameof(PromotionAction.CustomerParameter));

        string customerSegmentCondition =
            $"q[{actionKindField}]=={(int)PromotionActionKind.JoinInCustomerSegment}";
        form.ShowHide(actionKindField, customerSegmentCondition, nameof(PromotionAction.CustomerSegment));

        string rewardCondition =
            $"q[{actionKindField}]=={(int)PromotionActionKind.GrantReward}";
        form.ShowHide(actionKindField, rewardCondition, nameof(PromotionAction.Reward));

        string lotteryCondition =
            $"q[{actionKindField}]=={(int)PromotionActionKind.JoinLottery}";
        form.ShowHide(actionKindField, lotteryCondition, nameof(PromotionAction.Lottery));

        string externalApiCondition =
            $"q[{actionKindField}]=={(int)PromotionActionKind.CallExternalApi}";
        form.ShowHide(actionKindField, externalApiCondition,
            nameof(Groups.ExternalApiSettings),
            nameof(PromotionAction.ExternalApi),
            nameof(PromotionAction.PathParameterMappingsJson),
            nameof(PromotionAction.QueryParameterMappingsJson),
            nameof(PromotionAction.HeaderMappingsJson),
            nameof(PromotionAction.BodyTemplate), 
            nameof(PromotionAction.BodyContentType), 
            nameof(PromotionAction.ResponseMappingsJson));

        // نمایش AmountFormula برای عملیات‌هایی که نیاز به مقدار دارند
        string amountRelatedCondition =
            $"(q[{actionKindField}]!={(int)PromotionActionKind.JoinInCustomerSegment}) && " +
            $"(q[{actionKindField}]!={(int)PromotionActionKind.None})";
        form.ShowHide(actionKindField, amountRelatedCondition, 
            nameof(Groups.AmountSettings),
            nameof(PromotionAction.AmountFormula));

        string messageTemplateCondition =
            $"q[{nameof(PromotionAction.NotificationSendMethod)}]>{(int)PromotionNotificationSendMethod.None}";
        form.ShowHide(nameof(PromotionAction.NotificationSendMethod),
            messageTemplateCondition,
            nameof(PromotionAction.MessageTemplate), 
            nameof(Groups.NotificationSettings));*/
    }
}