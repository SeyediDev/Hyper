using System.Globalization;
using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public sealed record ChatMessageRequest(int ChatId, string MessageType, object Content);
public sealed record CreateChatRequest(int UserId);
public sealed record ChatMessagesQuery(int ChatId, int? MessageId = null, int Limit = 20,
    string Order = "desc", string? Compare = null);
public sealed record ChatsQuery(int Limit = 20, string OrderBy = "updated_at",
    string? Filter = null);

public interface IChatService
{
    Task<JsonElement> CreateMessageAsync(ChatMessageRequest request, CancellationToken ct = default);
    Task<JsonElement> CreateChatAsync(CreateChatRequest request, CancellationToken ct = default);
    Task<JsonElement> GetMessagesAsync(ChatMessagesQuery query, CancellationToken ct = default);
    Task<JsonElement> GetChatsAsync(ChatsQuery query, CancellationToken ct = default);
    Task<JsonElement> GetUnseenCountAsync(CancellationToken ct = default);
}

public sealed class ChatService(IBasalamHttpClient client, ILogger<ChatService>? logger = null) : IChatService
{
    public Task<JsonElement> CreateMessageAsync(ChatMessageRequest request, CancellationToken ct = default)
    {
        ValidateId(request.ChatId, "chatId");
        if (string.IsNullOrWhiteSpace(request.MessageType) || request.MessageType.Length > 40)
            throw Invalid("messageType");
        ArgumentNullException.ThrowIfNull(request.Content);
        logger?.LogInformation("Creating message in chat {ChatId}", request.ChatId);
        return client.PostAsync<JsonElement>($"/v1/chats/{request.ChatId}/messages", request, ct);
    }

    public Task<JsonElement> CreateChatAsync(CreateChatRequest request, CancellationToken ct = default)
    {
        ValidateId(request.UserId, "userId");
        return client.PostAsync<JsonElement>("/v1/chats", request, ct);
    }

    public Task<JsonElement> GetMessagesAsync(ChatMessagesQuery query, CancellationToken ct = default)
    {
        ValidateId(query.ChatId, "chatId");
        if (query.Limit is < 1 or > 100) throw Invalid("limit");
        var url = $"/v1/chats/{query.ChatId}/messages?limit={query.Limit}&order={Uri.EscapeDataString(query.Order)}";
        if (query.MessageId is { } id) { ValidateId(id, "messageId"); url += $"&message_id={id}"; }
        if (!string.IsNullOrWhiteSpace(query.Compare)) url += $"&cmp={Uri.EscapeDataString(query.Compare)}";
        return client.GetAsync<JsonElement>(url, ct);
    }

    public Task<JsonElement> GetChatsAsync(ChatsQuery query, CancellationToken ct = default)
    {
        if (query.Limit is < 1 or > 100) throw Invalid("limit");
        var url = $"/v1/chats?limit={query.Limit}&order_by={Uri.EscapeDataString(query.OrderBy)}";
        if (!string.IsNullOrWhiteSpace(query.Filter)) url += $"&filters={Uri.EscapeDataString(query.Filter)}";
        return client.GetAsync<JsonElement>(url, ct);
    }

    public Task<JsonElement> GetUnseenCountAsync(CancellationToken ct = default) =>
        client.GetAsync<JsonElement>("/v1/chats/unseen-count", ct);

    private static void ValidateId(int value, string field)
    {
        if (value <= 0) throw Invalid(field);
    }
    private static BasalamValidationError Invalid(string field) =>
        new(new Dictionary<string, IReadOnlyList<string>> { [field] = ["Value is invalid"] });
}
