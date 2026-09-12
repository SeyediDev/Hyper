using System.Security.Claims;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.AdminPanel.Web.Infrastructure;
using Microsoft.AspNetCore.DataProtection;

internal static class AccessChecks
{
    public static int Run()
    {
        const string issuer = "https://identity.example.test/realm/hyper";
        var count = 0;
        void Check(string name, bool result) { if (!result) throw new Exception(name); count++; Console.WriteLine("PASS " + name); }
        ClaimsPrincipal Principal(string type, string value, string claimIssuer = issuer, bool authenticated = true) =>
            new(new ClaimsIdentity([new Claim(type, value, ClaimValueTypes.String, claimIssuer)], authenticated ? "validated-token" : null));
        Check("subject identity preserves textual identifier", IntegrationMerchantClaims.Read(Principal("sub", "account-one"), issuer)?.SubjectId == "account-one");
        Check("missing trusted issuer denied", IntegrationMerchantClaims.Read(Principal("sub", "account-one"), null) is null);
        Check("unauthenticated claim denied", IntegrationMerchantClaims.Read(Principal("sub", "account-one", authenticated: false), issuer) is null);
        Check("foreign issuer denied", IntegrationMerchantClaims.Read(Principal("sub", "account-one", "https://other.example.test"), issuer) is null);
        Check("development memory issuer denied", IntegrationMerchantClaims.Read(Principal("sub", "account-one", "Club.Channel.Api"), issuer) is null);
        Check("admin userid is not merchant identity", IntegrationMerchantClaims.Read(Principal("userid", "123"), issuer) is null);
        Check("admin role does not create merchant identity", IntegrationMerchantClaims.Read(Principal(ClaimTypes.Role, "Admin"), issuer) is null);
        Check("mapped JWT subject accepted", IntegrationMerchantClaims.Read(Principal(ClaimTypes.NameIdentifier, "account-one"), issuer)?.SubjectId == "account-one");
        var mixed = Principal(ClaimTypes.NameIdentifier, "account-one");
        ((ClaimsIdentity)mixed.Identity!).AddClaim(new Claim(ClaimTypes.NameIdentifier, "admin-123"));
        Check("local admin claim cannot replace issuer subject", IntegrationMerchantClaims.Read(mixed, issuer)?.SubjectId == "account-one");
        ((ClaimsIdentity)mixed.Identity!).AddClaim(new Claim(ClaimTypes.NameIdentifier, "account-two", ClaimValueTypes.String, issuer));
        Check("ambiguous authenticated subjects denied", IntegrationMerchantClaims.Read(mixed, issuer) is null);
        Check("identifier whitespace denied", IntegrationMerchantClaims.Read(Principal("sub", "account-one "), issuer) is null);
        var scope = IntegrationConnectionScope.For([new(10, "tenant-a"), new(20, "tenant-b")]).Compile();
        Check("granted shop tenant pair allowed", scope(new() { ShopId = 10, TenantId = "tenant-a" }));
        Check("tenant mixing across shops denied", !scope(new() { ShopId = 10, TenantId = "tenant-b" }));
        Check("ungranted shop denied", !scope(new() { ShopId = 30, TenantId = "tenant-a" }));
        Check("empty grants deny all", !IntegrationConnectionScope.For([]).Compile()(new() { ShopId = 10, TenantId = "tenant-a" }));
        Check("tenantless shop gets deterministic scope", IntegrationConnectionScope.CanonicalTenant(10, null) == "shop:10");
        var now = DateTime.UtcNow;
        var simulation = new IntegrationAdminSimulation { AdminUserId = "admin-a", ShopId = 10, ExpiresAtUtc = now.AddMinutes(30) };
        Check("selected admin and displayed shop allowed", AdminSimulationBoundary.Allows(simulation, "admin-a", now, 10));
        Check("another admin cannot reuse selection", !AdminSimulationBoundary.Allows(simulation, "admin-b", now, 10));
        Check("different displayed shop denied", !AdminSimulationBoundary.Allows(simulation, "admin-a", now, 20));
        Check("expiry boundary denied", !AdminSimulationBoundary.Allows(simulation, "admin-a", simulation.ExpiresAtUtc, 10));
        Check("anonymous admin denied", !AdminSimulationBoundary.Allows(simulation, "", now, 10));
        simulation.EndedAtUtc = now;
        Check("ended or switched selection denied", !AdminSimulationBoundary.Allows(simulation, "admin-a", now, 10));
        var tickets = new AdminSimulationTickets(new EphemeralDataProtectionProvider());
        var selectionId = Guid.NewGuid();
        var ticket = tickets.Protect("admin-a", selectionId);
        Check("protected context restores selection", tickets.Read("admin-a", ticket) == selectionId);
        Check("another admin cannot decrypt context", tickets.Read("admin-b", ticket) is null);
        Check("tampered context rejected", tickets.Read("admin-a", "x" + ticket[1..]) is null);
        Check("raw simulation id is not a valid context", tickets.Read("admin-a", selectionId.ToString("N")) is null);
        Check("empty context rejected", tickets.Read("admin-a", null) is null);
        return count;
    }
}
