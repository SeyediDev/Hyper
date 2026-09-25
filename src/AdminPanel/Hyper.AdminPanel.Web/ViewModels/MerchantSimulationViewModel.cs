using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.AdminPanel.Web.ViewModels;

public sealed record MerchantSimulationViewModel(string AdminName, string? Search,
    IReadOnlyList<SimulationShop> Shops, IntegrationAdminSimulation? Selected, string? ContextTicket);

