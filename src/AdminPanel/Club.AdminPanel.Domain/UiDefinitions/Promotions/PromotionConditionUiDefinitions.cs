namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public class PromotionConditionUiDefinitions : SubCRUDDefinition<PromotionCondition>
{
    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(
            nameof(PromotionCondition.Promotion),
            nameof(PromotionCondition.Title),
            nameof(PromotionCondition.Type),
            nameof(PromotionCondition.EventChannel),
            nameof(PromotionCondition.EventType),
            nameof(PromotionCondition.PointLevel),
            nameof(PromotionCondition.Award),
            nameof(PromotionCondition.Product),
            nameof(PromotionCondition.MinimumCount),
            nameof(PromotionCondition.SequenceOrder)
        );
    }
    
    protected override void CUDFormsViewModel()
    {
        SubViewModel();
    }

    public override string SubjectId => "Sub";
    
    public override void SubIndexViewModel()
    {
        AddColumns(
            nameof(PromotionCondition.Title),
            nameof(PromotionCondition.Type),
            nameof(PromotionCondition.EventChannel),
            nameof(PromotionCondition.EventType),
            nameof(PromotionCondition.PointLevel),
            nameof(PromotionCondition.Award),
            nameof(PromotionCondition.Product),
            nameof(PromotionCondition.MinimumCount),
            nameof(PromotionCondition.SequenceOrder)
        );
    }

    public override void SubViewModel()
    {
        eControlTypeId control = eControlTypeId.Card;
        
        form.AddControl(control, "TypeGroup", "تنظیمات نوع شرط");
        {
            form.StartSubControls();
            AddFields(
                nameof(PromotionCondition.Promotion),
                nameof(PromotionCondition.Title),
                nameof(PromotionCondition.Type),
                nameof(PromotionCondition.EventChannel),
                nameof(PromotionCondition.EventType),
                nameof(PromotionCondition.PointLevel),
                nameof(PromotionCondition.Award),
                nameof(PromotionCondition.Product)
            );
            form.EndSubControls();
        }
        
        form.AddControl(control, "CampaignSettings", "تنظیمات کمپین (برای Campaign ها)");
        {
            form.StartSubControls();
            AddFields(
                nameof(PromotionCondition.MinimumCount),
                nameof(PromotionCondition.SequenceOrder),
                nameof(PromotionCondition.DependencyCondition),
                nameof(PromotionCondition.IsParallel)
            );
            form.EndSubControls();
        }
        
        form.AddControl(control, "ConditionSettings", "تنظیمات شرط");
        {
            form.StartSubControls();
            AddFields(
                nameof(PromotionCondition.ConditionGroup),
                nameof(PromotionCondition.Kind),
                nameof(PromotionCondition.Constraint),
                nameof(PromotionCondition.CompareWith),
                nameof(PromotionCondition.Point),
                nameof(PromotionCondition.EventTypeParameter),
                nameof(PromotionCondition.Value)
            );
            form.EndSubControls();
        }
    }

    protected override void UIRules(FormDefinition form)
    {
        const string typeField = nameof(PromotionCondition.Type);
        string eventCondition = $"q[{typeField}]=={(int)PromotionConditionType.Event}";
        form.ShowHide(typeField, eventCondition, nameof(PromotionCondition.EventChannel));
        form.ShowHide(typeField, eventCondition, nameof(PromotionCondition.EventType));

        string pointLevelCondition = $"q[{typeField}]=={(int)PromotionConditionType.UpgradePointLevel}";
        form.ShowHide(typeField, pointLevelCondition, nameof(PromotionCondition.PointLevel));

        string awardCondition =
            $"(q[{typeField}]=={(int)PromotionConditionType.PurchaseAward}) || (q[{typeField}]=={(int)PromotionConditionType.ConsumeAward})";
        form.ShowHide(typeField, awardCondition, nameof(PromotionCondition.Award));

        string productCondition = $"q[{typeField}]=={(int)PromotionConditionType.PurchaseProduct}";
        form.ShowHide(typeField, productCondition, nameof(PromotionCondition.Product));

        // Campaign settings - فقط برای Event conditions
        form.ShowHide(typeField, eventCondition, 
            nameof(PromotionCondition.MinimumCount),
            nameof(PromotionCondition.SequenceOrder),
            nameof(PromotionCondition.DependencyCondition),
            nameof(PromotionCondition.IsParallel),
            "CampaignSettings");

        const string kindField = nameof(PromotionCondition.Kind);
        string withFormulaCondition = $"q[{kindField}]=={(int)PromotionConditionKind.Formula}";
        string withCompareCondition = $"q[{kindField}]>={(int)PromotionConditionKind.EqualTo}";

        form.ShowHide(kindField, withFormulaCondition, nameof(PromotionCondition.Constraint));
        form.ShowHide(kindField, withCompareCondition, nameof(PromotionCondition.CompareWith));
        form.ShowHide(kindField, withCompareCondition, nameof(PromotionCondition.Value));

        const string compareWithField = nameof(PromotionCondition.CompareWith);
        string compareWithPointCondition =
            $"(q[{kindField}]>={(int)PromotionConditionKind.EqualTo}) && " +
            $"(q[{compareWithField}]=={(int)PromotionConditionCompareWith.Point})";
        form.ShowHide(kindField, compareWithPointCondition, nameof(PromotionCondition.Point));
        form.ShowHide(compareWithField, compareWithPointCondition, nameof(PromotionCondition.Point));

        string compareWithParameterCondition =
            $"(q[{kindField}]>={(int)PromotionConditionKind.EqualTo}) && " +
            $"(q[{compareWithField}]=={(int)PromotionConditionCompareWith.Parameter}) && " +
            $"({eventCondition})";
        form.ShowHide(kindField, compareWithParameterCondition, nameof(PromotionCondition.EventTypeParameter));
        form.ShowHide(compareWithField, compareWithParameterCondition, nameof(PromotionCondition.EventTypeParameter));
        form.ShowHide(typeField, compareWithParameterCondition, nameof(PromotionCondition.EventTypeParameter));

        form.FilterFormula(nameof(PromotionCondition.EventTypeParameter),
            nameof(PromotionCondition.EventType),
            $"{nameof(EventTypeParameter.EventTypeId)}==q[{nameof(PromotionCondition.EventType)}]");

        form.ShowHide(typeField, eventCondition, nameof(PromotionCondition.EventTypeParameter));
    }
}

