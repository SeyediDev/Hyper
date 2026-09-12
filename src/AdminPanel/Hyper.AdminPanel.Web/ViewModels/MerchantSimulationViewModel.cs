using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;

namespace Hyper.AdminPanel.Web.ViewModels;

public sealed record MerchantSimulationViewModel(string AdminName, string? Search,
    IReadOnlyList<SimulationShop> Shops, IntegrationAdminSimulation? Selected, string? ContextTicket);
