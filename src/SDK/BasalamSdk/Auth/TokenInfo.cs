namespace Basalam.SDK.Auth;

public record TokenInfo
{
    public required string AccessToken { get; init; }
    public required string TokenType { get; init; } = "Bearer";
    public int ExpiresIn { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string? RefreshToken { get; init; }
    public Scope Scopes { get; init; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt.AddMinutes(-5);
}
