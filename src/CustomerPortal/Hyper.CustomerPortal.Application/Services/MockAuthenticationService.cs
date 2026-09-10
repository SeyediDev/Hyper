using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hyper.CustomerPortal.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Hyper.CustomerPortal.Application.Services;

/// <summary>
/// Mock implementation برای تست - باید با implementation واقعی جایگزین شود
/// </summary>
public class MockAuthenticationService(ILogger<MockAuthenticationService> logger) : IAuthenticationService
{
    private static readonly Dictionary<string, string> _refreshTokens = [];

    public Task<TokenResult> GenerateTokenAsync(int customerId, string phoneNumber, CancellationToken cancellationToken = default)
    {
        // Mock JWT token generation
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ThisIsASecretKeyForMockAuthenticationService123456");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, customerId.ToString()),
                new Claim(ClaimTypes.MobilePhone, phoneNumber),
                new Claim("CustomerId", customerId.ToString())
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        var refreshToken = Guid.NewGuid().ToString();

        _refreshTokens[refreshToken] = customerId.ToString();
        
        logger.LogInformation("Mock: Generated token for customer {CustomerId}", customerId);

        return Task.FromResult(new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600
        });
    }

    public Task<TokenResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (!_refreshTokens.TryGetValue(refreshToken, out var customerIdStr))
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var customerId = int.Parse(customerIdStr);
        logger.LogInformation("Mock: Refreshed token for customer {CustomerId}", customerId);

        // Generate new tokens
        return GenerateTokenAsync(customerId, string.Empty, cancellationToken);
    }

    public Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        _refreshTokens.Remove(refreshToken);
        logger.LogInformation("Mock: Revoked refresh token");
        return Task.CompletedTask;
    }
}

