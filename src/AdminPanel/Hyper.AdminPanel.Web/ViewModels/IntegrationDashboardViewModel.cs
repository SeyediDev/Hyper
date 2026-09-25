using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

namespace Hyper.AdminPanel.Web.ViewModels;

public sealed record IntegrationDashboardViewModel(IntegrationAdminSimulation? Selected, IntegrationDashboardSnapshot? Statistics);

