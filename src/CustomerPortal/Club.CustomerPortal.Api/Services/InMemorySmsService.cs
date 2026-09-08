using System.Collections.Concurrent;
using System.Security.Cryptography;
using Neo.Domain.Features.Sms;

namespace Hyper.CustomerPortal.Api.Services;

internal sealed class InMemorySmsService(
    ILogger<InMemorySmsService> logger
) : ISmsService, IOtpService
{
    private readonly ConcurrentDictionary<string, (byte[] Seed, DateTimeOffset ExpireAt)> _otpStore = new();

    public Task SendAsync(Neo.Domain.Features.Sms.Dto.SmsDto model, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("SMS => {Mobile}: {Message}", model.mobile, model.message);
        return Task.CompletedTask;
    }

    public Task SendOtpAsync(Neo.Domain.Features.Sms.Dto.OtpSmsDto model, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("OTP SMS => {Mobile}: {Template}", model.mobile, model.message);
        return Task.CompletedTask;
    }

    public byte[] GetNewOtpSeed()
    {
        byte[] buffer = RandomNumberGenerator.GetBytes(6);
        return buffer;
    }

    public Task<DateTimeOffset> SendAsync(string mobile, byte[] seedBytes, string messageTemplate)
    {
        DateTimeOffset expireAt = DateTimeOffset.UtcNow.AddMinutes(5);
        _otpStore[mobile] = (seedBytes, expireAt);
        logger.LogInformation("Generated OTP for {Mobile}", mobile);
        return Task.FromResult(expireAt);
    }

    public bool Verify(byte[] seedBytes, string otp)
    {
        string expected = ConvertSeedToCode(seedBytes);
        return string.Equals(expected, otp, StringComparison.Ordinal);
    }

    public Task<TimeSpan?> OtpTimeout(string mobile)
    {
        if (_otpStore.TryGetValue(mobile, out var entry))
        {
            TimeSpan remaining = entry.ExpireAt - DateTimeOffset.UtcNow;
            if (remaining > TimeSpan.Zero)
            {
                return Task.FromResult<TimeSpan?>(remaining);
            }
        }
        return Task.FromResult<TimeSpan?>(null);
    }

    private static string ConvertSeedToCode(byte[] seed)
    {
        int value = Math.Abs(BitConverter.ToInt32(seed, 0));
        return (value % 1000000).ToString("D6");
    }
}

