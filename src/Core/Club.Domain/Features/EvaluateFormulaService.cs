using Neo.Bpms.Domain.Extensions;

namespace Club.Domain.Features;
public interface IEvaluateFormulaService
{
    object Evaluate(string formula);
    Task<object> Evaluate(int? eventTypeId, string formula, Dictionary<string, string> parameters, CancellationToken cancellationToken);
}
internal class EvaluateFormulaService(IEventService eventService) : IEvaluateFormulaService
{
    public object Evaluate(string formula)
    {
        try
        {
            NCalc.Expression expression = new(formula);
            var result = expression.Evaluate();
            return result;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid formula: {formula}", ex);
        }
    }

    // Usage with variables
    public async Task<object> Evaluate(int? eventTypeId, string formula, Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        var expression = new NCalc.Expression(formula);

        foreach (var parameter in parameters)
        {
            EventTypeParameter? param = eventTypeId!=null? await eventService.GetParameterAsync(eventTypeId.Value, parameter.Key, false, cancellationToken):null;
            switch (param?.ParameterType)
            {
                case ParameterType.String:
                    expression.Parameters[parameter.Key] = parameter.Value;
                    break;
                case ParameterType.Long:
                    expression.Parameters[parameter.Key] = parameter.Value.ToInt64OrDefault();
                    break;
                case ParameterType.Float:
                    expression.Parameters[parameter.Key] = parameter.Value.ToFloatOrDefault();
                    break;
                case ParameterType.DateOnly:
                    expression.Parameters[parameter.Key] = parameter.Value.ToDateTimeOrDefault().Date;
                    break;
                case ParameterType.DateTime:
                    expression.Parameters[parameter.Key] = parameter.Value.ToDateTimeOrDefault();
                    break;
                case ParameterType.TimeOnly:
                    expression.Parameters[parameter.Key] = parameter.Value.ToTimeSpan();
                    break;
                default:
                    expression.Parameters[parameter.Key] = parameter.Value;
                    break;
            }
        }

        return expression.Evaluate();
    }
}
