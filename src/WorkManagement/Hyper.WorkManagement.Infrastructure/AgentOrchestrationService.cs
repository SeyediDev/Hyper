using System.Net.Http.Headers;
using System.Net.Http.Json;
using Hyper.WorkManagement.Contracts;
using Hyper.WorkManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hyper.WorkManagement.Infrastructure;

public sealed class AgentHarnessOptions
{
    public bool Enabled { get; set; }
    public string Provider { get; set; } = "codex";
    public string? Endpoint { get; set; }
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 60;
}

public sealed record AgentHarnessDispatchRequest(string RunId, string TaskKey, string Title, string? Description,
    string RoleKey, string AgentProfileKey, string Provider, string? Model, string? Instructions, string? SkillPath,
    string? Branch, string CallbackUrl);
public sealed record AgentHarnessDispatchResult(bool Accepted, string? ExternalRunId, string? Error);
public interface IAgentHarnessDispatcher
{
    Task<AgentHarnessDispatchResult> DispatchAsync(AgentHarnessDispatchRequest request, CancellationToken ct = default);
}

public sealed class HttpAgentHarnessDispatcher(HttpClient client, IOptions<AgentHarnessOptions> options) : IAgentHarnessDispatcher
{
    public async Task<AgentHarnessDispatchResult> DispatchAsync(AgentHarnessDispatchRequest request, CancellationToken ct = default)
    {
        var settings = options.Value;
        if (!settings.Enabled || !Uri.TryCreate(settings.Endpoint, UriKind.Absolute, out var endpoint))
            return new(false, null, "HarnessNotConfigured");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(Math.Clamp(settings.TimeoutSeconds, 5, 300)));
        using var message = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = JsonContent.Create(request) };
        if (!string.IsNullOrWhiteSpace(settings.ApiKey)) message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        try
        {
            using var response = await client.SendAsync(message, timeout.Token);
            if (!response.IsSuccessStatusCode) return new(false, null, $"HarnessHttp{(int)response.StatusCode}");
            var result = await response.Content.ReadFromJsonAsync<HarnessResponse>(cancellationToken: timeout.Token);
            return new(true, result?.ExternalRunId, null);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return new(false, null, "HarnessTimeout"); }
        catch (HttpRequestException) { return new(false, null, "HarnessTransport"); }
    }
    private sealed record HarnessResponse(string? ExternalRunId);
}

public sealed class AgentOrchestrationService(WorkManagementContext db, IAgentHarnessDispatcher harness, IOptions<AgentHarnessOptions> options) : IAgentOrchestrationService
{
    public async Task<AgentOrchestrationSnapshot> GetAsync(CancellationToken ct = default)
    {
        var profiles = await db.AgentProfiles.AsNoTracking().OrderBy(x => x.RoleKey).ThenBy(x => x.Key).ToListAsync(ct);
        var transitions = await db.WorkflowTransitions.AsNoTracking().OrderBy(x => x.Key).ToListAsync(ct);
        var runs = await RunsQuery(null, ct);
        return new(profiles.Select(Profile).ToList(), transitions.Select(Transition).ToList(), runs);
    }

    public async Task<AgentProfileView?> CreateProfileAsync(CreateAgentProfileRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.RoleKey)
            || await db.AgentProfiles.AnyAsync(x => x.Key == request.Key.Trim(), ct)) return null;
        var now = DateTime.UtcNow;
        var profile = new AgentProfile { Key = request.Key.Trim(), Name = request.Name.Trim(), RoleKey = request.RoleKey.Trim(), Provider = request.Provider.Trim(), Model = request.Model?.Trim(), Instructions = request.Instructions, SkillPath = request.SkillPath, IsEnabled = request.IsEnabled, CreatedAtUtc = now, UpdatedAtUtc = now };
        db.AgentProfiles.Add(profile); await db.SaveChangesAsync(ct); return Profile(profile);
    }

    public async Task<WorkflowTransitionView?> CreateTransitionAsync(CreateWorkflowTransitionRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.FromRoleKey) || string.IsNullOrWhiteSpace(request.NextRoleKey)
            || await db.WorkflowTransitions.AnyAsync(x => x.Key == request.Key.Trim(), ct)) return null;
        var transition = new WorkflowTransition { Key = request.Key.Trim(), ProjectKey = request.ProjectKey?.Trim(), Domain = request.Domain?.Trim(), FromStatus = request.FromStatus, TriggerStatus = request.TriggerStatus, FromRoleKey = request.FromRoleKey.Trim(), NextRoleKey = request.NextRoleKey.Trim(), NextStatus = request.NextStatus, RequireTests = request.RequireTests, RequireCommits = request.RequireCommits, AutoDispatch = request.AutoDispatch, IsEnabled = request.IsEnabled, UpdatedAtUtc = DateTime.UtcNow };
        db.WorkflowTransitions.Add(transition); await db.SaveChangesAsync(ct); return Transition(transition);
    }

    public Task<IReadOnlyList<AgentRunView>> GetRunsAsync(long workItemId, CancellationToken ct = default) => RunsQuery(workItemId, ct);

    public async Task OnStatusChangedAsync(long workItemId, WorkItemStatus fromStatus, WorkItemStatus toStatus, CancellationToken ct = default)
    {
        var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == workItemId, ct);
        if (item is null) return;
        var transition = await db.WorkflowTransitions.AsNoTracking().FirstOrDefaultAsync(x => x.IsEnabled && x.FromStatus == fromStatus && x.TriggerStatus == toStatus
            && (x.FromRoleKey == "*" || x.FromRoleKey == item.OwnerRole)
            && (x.ProjectKey == null || x.ProjectKey == item.Project!.Key) && (x.Domain == null || x.Domain == item.Domain), ct);
        if (transition is null || (transition.RequireTests && !await db.WorkItemTestEvidence.AnyAsync(x => x.WorkItemId == workItemId, ct))
            || (transition.RequireCommits && !await db.WorkItemCommits.AnyAsync(x => x.WorkItemId == workItemId, ct))) return;
        var profile = await db.AgentProfiles.FirstOrDefaultAsync(x => x.IsEnabled && x.RoleKey == transition.NextRoleKey, ct);
        if (profile is null || await db.AgentRuns.AnyAsync(x => x.WorkItemId == workItemId && x.TargetRoleKey == transition.NextRoleKey && (x.Status == "Queued" || x.Status == "Dispatched"), ct)) return;
        var run = new AgentRun { WorkItemId = item.Id, AgentProfileId = profile.Id, TargetRoleKey = profile.RoleKey, Status = "Queued", Branch = item.Branch, Prompt = item.Description };
        db.AgentRuns.Add(run); item.OwnerRole = transition.NextRoleKey; item.OwnerAgent = profile.Key; item.UpdatedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct);
        if (transition.AutoDispatch) await DispatchRunAsync(run, item, profile, ct);
    }

    public async Task<AgentRunView?> DispatchNextAsync(long workItemId, CancellationToken ct = default)
    {
        var run = await db.AgentRuns.Where(x => x.WorkItemId == workItemId && x.Status == "Queued").OrderBy(x => x.Id).FirstOrDefaultAsync(ct);
        if (run is null) return null;
        var item = await db.WorkItems.Include(x => x.Project).SingleOrDefaultAsync(x => x.Id == workItemId, ct);
        var profile = await db.AgentProfiles.SingleOrDefaultAsync(x => x.Id == run.AgentProfileId, ct);
        if (item is null || profile is null) return null;
        await DispatchRunAsync(run, item, profile, ct); return await RunsQuery(workItemId, ct).ContinueWith(x => x.Result.FirstOrDefault(y => y.Id == run.Id), ct);
    }

    public async Task<AgentRunView?> CompleteRunAsync(long runId, AgentRunCallbackRequest request, string? harnessKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(options.Value.ApiKey) || !string.Equals(options.Value.ApiKey, harnessKey, StringComparison.Ordinal)) return null;
        var status = request.Status.Trim();
        if (status is not ("Running" or "Succeeded" or "Failed" or "NeedsInput")) return null;
        var run = await db.AgentRuns.SingleOrDefaultAsync(x => x.Id == runId, ct);
        if (run is null) return null;
        run.Status = status; run.ExternalRunId = request.ExternalRunId ?? run.ExternalRunId; run.LastError = request.Error;
        run.CompletedAtUtc = status is "Succeeded" or "Failed" or "NeedsInput" ? DateTime.UtcNow : null;
        await db.SaveChangesAsync(ct);
        return (await RunsQuery(run.WorkItemId, ct)).FirstOrDefault(x => x.Id == runId);
    }

    private async Task DispatchRunAsync(AgentRun run, WorkItem item, AgentProfile profile, CancellationToken ct)
    {
        run.Status = "Dispatched"; run.StartedAtUtc = DateTime.UtcNow; await db.SaveChangesAsync(ct);
        var result = await harness.DispatchAsync(new(run.Id.ToString(), item.Key, item.Title, item.Description, profile.RoleKey, profile.Key, profile.Provider, profile.Model, profile.Instructions, profile.SkillPath, run.Branch, "/api/work-management/v1/orchestration/runs/" + run.Id + "/callback"), ct);
        if (result.Accepted) { run.ExternalRunId = result.ExternalRunId; run.LastError = null; }
        else { run.Status = "NeedsConfiguration"; run.LastError = result.Error; run.CompletedAtUtc = DateTime.UtcNow; }
        await db.SaveChangesAsync(ct);
    }

    private async Task<IReadOnlyList<AgentRunView>> RunsQuery(long? workItemId, CancellationToken ct) => await (from run in db.AgentRuns.AsNoTracking() join item in db.WorkItems.AsNoTracking() on run.WorkItemId equals item.Id join profile in db.AgentProfiles.AsNoTracking() on run.AgentProfileId equals profile.Id where workItemId == null || run.WorkItemId == workItemId orderby run.Id descending select new AgentRunView(run.Id, run.WorkItemId, item.Key, run.AgentProfileId, profile.Key, run.TargetRoleKey, run.Status, run.ExternalRunId, run.Branch, run.LastError, run.CreatedAtUtc, run.StartedAtUtc, run.CompletedAtUtc)).Take(100).ToListAsync(ct);
    private static AgentProfileView Profile(AgentProfile x) => new(x.Id, x.Key, x.Name, x.RoleKey, x.Provider, x.Model, x.Instructions, x.SkillPath, x.IsEnabled);
    private static WorkflowTransitionView Transition(WorkflowTransition x) => new(x.Id, x.Key, x.ProjectKey, x.Domain, x.FromStatus, x.TriggerStatus, x.FromRoleKey, x.NextRoleKey, x.NextStatus, x.RequireTests, x.RequireCommits, x.AutoDispatch, x.IsEnabled);
}
