using NCalc;

namespace Hyper.Domain.Features.Promotions;

public interface IEvaluateFormulaService
{
    object Evaluate(string? formula);
    bool Check(string? condition, AttributesValues? attributesValues = null);
    object Evaluate(string? formula, AttributesValues? attributesValues);
}

internal class EvaluateFormulaService : IEvaluateFormulaService
{
    public object Evaluate(string? formula)
    {
        return Evaluate(formula, null);
    }

    // Usage with variables
    public object Evaluate(string? formula, AttributesValues? attributesValues)
    {
        try
        {
            if (string.IsNullOrEmpty(formula))
                return null!;
            formula = ConvertToNCalcFormula(formula);
            var expression = new Expression(formula);
            AddCustomFunctions(expression);
            if (attributesValues != null)
            {
                foreach (var attributesValue in attributesValues)
                {
                    if (attributesValue.Value is AttributesValues dicValue)
                    {
                        foreach (var dicValueItem in dicValue)
                        {
                            expression.Parameters[$"{attributesValue.Key}_{dicValueItem.Key}"] = dicValueItem.Value;
                        }
                    }
                    else
                    {
                        expression.Parameters[attributesValue.Key] = attributesValue.Value;
                    }
                }
            }

            return expression.Evaluate();
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid formula: {formula}", ex);
        }
    }

    private static string ConvertToNCalcFormula(string formula)
    {
        var values = typeof(AttributeArea).GetEnumNames();
        foreach (var value in values)
        {
            formula = ConvertToNCalcFormula(value, formula);
        }
        return formula;
    }

    /// <summary>
    /// اضافه کردن Custom Functions برای توابع C#
    /// </summary>
    private static void AddCustomFunctions(Expression expression)
    {
        // تابع Math
        expression.EvaluateFunction += (name, args) =>
        {
            switch (name.ToLower())
            {
                case "abs":
                    args.Result = Math.Abs(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
                case "max":
                    args.Result = Math.Max(
                        Convert.ToDouble(args.Parameters[0].Evaluate()),
                        Convert.ToDouble(args.Parameters[1].Evaluate()));
                    break;
                case "min":
                    args.Result = Math.Min(
                        Convert.ToDouble(args.Parameters[0].Evaluate()),
                        Convert.ToDouble(args.Parameters[1].Evaluate()));
                    break;
                case "round":
                    args.Result = Math.Round(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
                case "floor":
                    args.Result = Math.Floor(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
                case "ceiling":
                    args.Result = Math.Ceiling(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
            }
        };

        // تابع String
        expression.EvaluateFunction += (name, args) =>
        {
            switch (name.ToLower())
            {
                case "contains":
                    var str = args.Parameters[0].Evaluate()?.ToString() ?? "";
                    var substr = args.Parameters[1].Evaluate()?.ToString() ?? "";
                    args.Result = str.Contains(substr, StringComparison.OrdinalIgnoreCase);
                    break;
                case "startswith":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "")
                        .StartsWith(args.Parameters[1].Evaluate()?.ToString() ?? "", StringComparison.OrdinalIgnoreCase);
                    break;
                case "endswith":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "")
                        .EndsWith(args.Parameters[1].Evaluate()?.ToString() ?? "", StringComparison.OrdinalIgnoreCase);
                    break;
                case "tolower":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "").ToLower();
                    break;
                case "toupper":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "").ToUpper();
                    break;
            }
        };

        // تابع DateTime
        expression.EvaluateFunction += (name, args) =>
        {
            switch (name.ToLower())
            {
                case "now":
                    args.Result = DateTime.UtcNow;
                    break;
                case "today":
                    args.Result = DateTime.Today;
                    break;
                case "year":
                    args.Result = ((DateTime)args.Parameters[0].Evaluate()).Year;
                    break;
                case "month":
                    args.Result = ((DateTime)args.Parameters[0].Evaluate()).Month;
                    break;
                case "day":
                    args.Result = ((DateTime)args.Parameters[0].Evaluate()).Day;
                    break;
                case "datediff":
                    var date1 = (DateTime)args.Parameters[0].Evaluate();
                    var date2 = (DateTime)args.Parameters[1].Evaluate();
                    args.Result = (date2 - date1).TotalDays;
                    break;
            }
        };
    }

    public bool Check(string? condition, AttributesValues? attributesValues = null)
    {
        // اگر شرط خالی یا null باشد، به معنای بدون شرط اضافی است و همیشه true برمی‌گرداند
        // این منطق برای محرک‌هایی که نیاز به شرط اضافی ندارند استفاده می‌شود
        if (string.IsNullOrWhiteSpace(condition))
            return true;

        try
        {
            var result = Evaluate(condition, attributesValues);
            return Convert.ToBoolean(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error evaluating condition: {condition}", ex);
        }
    }

    /// <summary>
    /// تبدیل فرمول به فرم NCalc
    /// Customer.Age -> Customer_Age
    /// CustomerPoint.Gold          -> CustomerPoint_Gold
    /// CustomerPoint.Gold.LevelId  -> CustomerPoint_Gold_LevelId
    /// CustomerPoint.Gold.Level    -> CustomerPoint_Gold_Level
    /// Tenant.RegularAge -> Tenant_RegularAge
    /// Event.Cost -> Event_Cost
    /// Product.Price -> Product_Price
    /// </summary>
    private static string ConvertToNCalcFormula(string prefix, string formula)
    {
        var converted = formula;

        // تبدیل prefix.*
        converted = System.Text.RegularExpressions.Regex.Replace(
            converted,
            @$"{prefix}\.(\w+)",
            match => $"{prefix}_{match.Groups[1].Value.Replace('.','_')}");
        return converted;
    }
}