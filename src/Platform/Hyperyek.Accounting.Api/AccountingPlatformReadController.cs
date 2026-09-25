using Hyperyek.Accounting.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyperyek.Accounting.Api;

[ApiController]
[Authorize]
[Route("api/hyperyek/v1/accounting/platform")]
public sealed class AccountingPlatformReadController(IAccountingPlatformReadHandler handler) : ControllerBase
{
    [HttpGet("shops")]
    public Task<IReadOnlyList<AccountingShopRead>> SearchShops([FromQuery] string? search, CancellationToken ct) =>
        handler.SearchShopsAsync(search, ct);

    [HttpPost("shops/by-ids")]
    public Task<IReadOnlyList<AccountingShopRead>> GetShops([FromBody] IReadOnlyCollection<int> shopIds, CancellationToken ct) =>
        handler.GetShopsAsync(shopIds, ct);

    [HttpGet("shops/{shopId:int}")]
    public async Task<IActionResult> GetShop(int shopId, CancellationToken ct) =>
        (await handler.GetShopAsync(shopId, ct)) is { } shop ? Ok(shop) : NotFound();

    [HttpGet("shops/{shopId:int}/products")]
    public Task<IReadOnlyList<AccountingProductRead>> GetProducts(int shopId, [FromQuery] string tenantId,
        CancellationToken ct) => handler.GetProductsAsync(new(shopId, tenantId), ct);
}
