namespace Hyper.CustomerPortal.Application.Interfaces;

/// <summary>
/// سرویس احراز هویت برای مدیریت JWT tokens و authentication
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// تولید JWT token برای مشتری
    /// </summary>
    Task<TokenResult> GenerateTokenAsync(int customerId, string phoneNumber, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// تایید و تجدید Refresh Token
    /// </summary>
    Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// لغو Refresh Token (در logout)
    /// </summary>
    Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}

public record TokenResult
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public int ExpiresIn { get; init; } = 3600; // 1 hour default
}

