using Hyper.Integration.Contracts;
namespace Hyper.AdminPanel.Web.ViewModels;
public sealed record ProductReadinessViewModel(long? ConnectionId, ProductTransferDirection Direction, int Skip,
    string ContextTicket, string ShopName, ProductPreparationPolicy Policy, ProductPreparationBoard? Board);
