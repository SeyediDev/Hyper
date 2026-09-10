namespace Hyper.Domain.Constants;

public class HyperRoles : Neo.Domain.Constants.Roles
{
    public static string Manager => nameof(Manager);
    public static string MarketingManager => nameof(MarketingManager);
    public static string FinanceManager => nameof(FinanceManager);
    public static string Analyst => nameof(Analyst);
    public static string CallCenterSupport => nameof(CallCenterSupport);
    public static string CallCenterManager => nameof(CallCenterManager);

    public static List<string> RolesManager => [Admin, Manager, CallCenterManager];
	public static List<string> RolesAnalyst => [Admin, Manager, Analyst, CallCenterManager];
    public static List<string> RolesCallCenter => [Admin, CallCenterManager, CallCenterSupport];
    public static List<string> RolesCallCenterManager => [Admin, CallCenterManager];
}
