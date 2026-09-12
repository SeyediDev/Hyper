using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;

namespace Hyper.AdminPanel.Web.ViewModels;

public sealed record IntegrationDashboardViewModel(IntegrationAdminSimulation? Selected, IntegrationDashboardSnapshot? Statistics);
