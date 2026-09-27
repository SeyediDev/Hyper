using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface IStoryService
{
    Task<JsonElement> GetMyStoriesAsync(int? count = null, bool? activeOnly = null, int? lastId = null, CancellationToken ct = default);
    Task<JsonElement> CreateStoryAsync(object request, CancellationToken ct = default);
    Task<JsonElement> GetDiscoveryAsync(StoryDiscoveryQuery? query = null, CancellationToken ct = default);
    Task<JsonElement> CreateReelAsync(object request, CancellationToken ct = default);
    Task<JsonElement> GetMyReelsAsync(int? limit = null, int? lastIdx = null, bool? isConfirmed = null, string? statusFilter = null, CancellationToken ct = default);
    Task<JsonElement> GetUserReelsAsync(int userId, int? limit = null, int? lastIdx = null, CancellationToken ct = default);
    Task<JsonElement> UpdateReelAsync(int reelId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteReelAsync(int reelId, CancellationToken ct = default);
    Task<JsonElement> LikeReelAsync(int reelId, object request, CancellationToken ct = default);
    Task<JsonElement> GetHashtagFeedAsync(string hashtag, int? count = null, int? lastId = null, CancellationToken ct = default);
}

public sealed record StoryDiscoveryQuery(string? DeviceId = null, int? CityId = null,
    string? CategoryIds = null, int? NextIndex = null, int? Count = null,
    bool? Regenerate = null, bool? SkipImpression = null, bool? Refresh = null);

public sealed class StoryService(IBasalamHttpClient client) : IStoryService
{
    public Task<JsonElement> GetMyStoriesAsync(int? count = null, bool? activeOnly = null, int? lastId = null, CancellationToken ct = default) =>
        Get("/v1/users/me/stories" + Query(("count", count), ("active_only", activeOnly), ("last_id", lastId)), ct);
    public Task<JsonElement> CreateStoryAsync(object request, CancellationToken ct = default) => Post("/v1/stories", request, ct);
    public Task<JsonElement> GetDiscoveryAsync(StoryDiscoveryQuery? query = null, CancellationToken ct = default)
    {
        var q = query ?? new StoryDiscoveryQuery();
        return Get("/v1/stories/discovery" + Query(("device_id", q.DeviceId), ("city_id", q.CityId),
            ("category_ids", q.CategoryIds), ("next_idx", q.NextIndex), ("count", q.Count),
            ("regenerate", q.Regenerate), ("skip_impression", q.SkipImpression), ("refresh", q.Refresh)), ct);
    }
    public Task<JsonElement> CreateReelAsync(object request, CancellationToken ct = default) => Post("/v1/reels", request, ct);
    public Task<JsonElement> GetMyReelsAsync(int? limit = null, int? lastIdx = null, bool? isConfirmed = null, string? statusFilter = null, CancellationToken ct = default) =>
        Get("/v1/users/me/reels" + Query(("limit", limit), ("last_idx", lastIdx), ("is_confirmed", isConfirmed), ("status_filter", statusFilter)), ct);
    public Task<JsonElement> GetUserReelsAsync(int userId, int? limit = null, int? lastIdx = null, CancellationToken ct = default) =>
        Get($"/v1/users/{Id(userId, "userId")}/reels" + Query(("limit", limit), ("last_idx", lastIdx)), ct);
    public Task<JsonElement> UpdateReelAsync(int reelId, object request, CancellationToken ct = default) => Put($"/v1/reels/{Id(reelId, "reelId")}", request, ct);
    public Task<JsonElement> DeleteReelAsync(int reelId, CancellationToken ct = default) => Delete($"/v1/reels/{Id(reelId, "reelId")}", ct);
    public Task<JsonElement> LikeReelAsync(int reelId, object request, CancellationToken ct = default) => Post($"/v1/reels/{Id(reelId, "reelId")}/likes", request, ct);
    public Task<JsonElement> GetHashtagFeedAsync(string hashtag, int? count = null, int? lastId = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(hashtag) || hashtag.Length > 100) throw Invalid("hashtag");
        return Get("/v1/feeds/hashtags" + Query(("hashtag", hashtag), ("count", count), ("last_id", lastId)), ct);
    }

    private Task<JsonElement> Get(string path, CancellationToken ct) => client.GetAsync<JsonElement>(path, ct);
    private Task<JsonElement> Post(string path, object request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        return client.PostAsync<JsonElement>(path, request, ct);
    }
    private Task<JsonElement> Put(string path, object request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        return client.PutAsync<JsonElement>(path, request, ct);
    }
    private Task<JsonElement> Delete(string path, CancellationToken ct) => client.DeleteAsync<JsonElement>(path, ct);
    private static int Id(int value, string field)
    {
        if (value <= 0) throw Invalid(field);
        return value;
    }
    private static string Query(params (string Name, object? Value)[] values)
    {
        var parts = values.Where(x => x.Value is not null && !string.IsNullOrWhiteSpace(x.Value.ToString()))
            .Select(x => Uri.EscapeDataString(x.Name) + "=" + Uri.EscapeDataString(x.Value!.ToString()!));
        var query = string.Join('&', parts);
        return query.Length == 0 ? string.Empty : "?" + query;
    }
    private static BasalamValidationError Invalid(string field) =>
        new(new Dictionary<string, IReadOnlyList<string>> { [field] = ["Value is invalid"] });
}
