using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace Hyper.AdminPanel.Web.Infrastructure;

public sealed class AdminSimulationTickets(IDataProtectionProvider protection)
{
    public string Protect(string adminId, Guid id) =>
        protection.CreateProtector("Hyper.AdminMerchantSimulation.v1", adminId).Protect(id.ToString("N"));

    public Guid? Read(string adminId, string? ticket)
    {
        if (string.IsNullOrWhiteSpace(ticket) || ticket.Length > 2048) return null;
        try
        {
            var value = protection.CreateProtector("Hyper.AdminMerchantSimulation.v1", adminId).Unprotect(ticket);
            return Guid.TryParseExact(value, "N", out var id) ? id : null;
        }
        catch (CryptographicException) { return null; }
    }
}
