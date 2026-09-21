using BasalamSandbox;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<SqlServerDemoStore>();

var app = builder.Build();
var store = app.Services.GetRequiredService<SqlServerDemoStore>();
await store.InitializeAsync(app.Lifetime.ApplicationStopping);

app.MapGet("/", () => Results.Redirect("/sandbox/vendor"));

app.MapGet("/accounts/sso", async (HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    var clientId = request.Query["client_id"].ToString();
    var redirectUri = request.Query["redirect_uri"].ToString();
    var scope = request.Query["scope"].ToString();
    var state = request.Query["state"].ToString();
    if (!db.IsClient(clientId) || !Uri.TryCreate(redirectUri, UriKind.Absolute, out _))
        return Results.Content(Html.Error("درخواست OAuth دمو معتبر نیست؛ client_id یا redirect_uri را بررسی کنید."), "text/html; charset=utf-8");

    var vendors = await db.ListVendorsAsync(ct);
    return Results.Content(Html.Consent(clientId, redirectUri, scope, state, vendors), "text/html; charset=utf-8");
});

app.MapPost("/accounts/sso/consent", async (HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    var form = await request.ReadFormAsync(ct);
    var clientId = form["client_id"].ToString();
    var redirectUri = form["redirect_uri"].ToString();
    var scope = form["scope"].ToString();
    var state = form["state"].ToString();
    var vendorId = long.TryParse(form["vendor_id"], out var parsed) ? parsed : 0;
    if (!db.IsClient(clientId) || !Uri.TryCreate(redirectUri, UriKind.Absolute, out _) || !await db.VendorExistsAsync(vendorId, ct))
        return Results.BadRequest("درخواست OAuth دمو معتبر نیست.");

    var code = await db.CreateAuthorizationCodeAsync(clientId, redirectUri, scope, state, vendorId, ct);
    var target = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(redirectUri, new Dictionary<string, string?>
    {
        ["code"] = code, ["state"] = state
    });
    return Results.Redirect(target);
});

app.MapPost("/oauth/token", async (HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    if (!request.HasFormContentType) return Results.BadRequest(new { error = "invalid_request", error_description = "form-urlencoded body required" });
    var form = await request.ReadFormAsync(ct);
    var grant = form["grant_type"].ToString();
    if (grant == "authorization_code")
    {
        var token = await db.ExchangeCodeAsync(form["code"].ToString(), form["client_id"].ToString(), form["client_secret"].ToString(), form["redirect_uri"].ToString(), ct);
        return token is null ? Results.BadRequest(new { error = "invalid_grant" }) : Results.Json(token);
    }
    if (grant == "refresh_token")
    {
        var token = await db.RefreshAsync(form["refresh_token"].ToString(), form["client_id"].ToString(), form["client_secret"].ToString(), ct);
        return token is null ? Results.BadRequest(new { error = "invalid_grant" }) : Results.Json(token);
    }
    return Results.BadRequest(new { error = "unsupported_grant_type" });
});

app.MapGet("/v3/users/me", async (HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    var vendor = await db.VendorForTokenAsync(request, ct);
    return vendor is null ? Results.Unauthorized() : Results.Json(vendor);
});

app.MapGet("/v3/vendors/{vendorId:long}/products", async (long vendorId, HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    if (!await db.TokenAllowsVendorAsync(request, vendorId, ct)) return Results.Unauthorized();
    return Results.Json(new { data = await db.ListProductsAsync(vendorId, ct), has_more = false });
});

app.MapGet("/v1/vendors/{vendorId:long}/products", async (long vendorId, HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    if (!await db.TokenAllowsVendorAsync(request, vendorId, ct)) return Results.Unauthorized();
    var items = await db.ListProductsAsync(vendorId, ct);
    return Results.Json(new
    {
        data = await Task.WhenAll(items.Select(async x => new { id = x.Id, vendor_id = vendorId, name = x.Name, sku = x.Sku, price = x.Price, stock = x.Stock, variants = (object)await db.ListVariantsAsync(x.Id, ct) })),
        total = items.Count, page = 1, per_page = 100, total_pages = 1, has_more = false
    });
});

app.MapGet("/v1/products/{productId:long}", async (long productId, HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    if (!await db.TokenValidAsync(request, ct)) return Results.Unauthorized();
    var item = await db.GetProductAsync(productId, ct);
    return item is null ? Results.NotFound() : Results.Json(new { id = item.Id, vendor_id = item.VendorId, name = item.Name, sku = item.Sku, price = item.Price, stock = item.Stock, variants = await db.ListVariantsAsync(item.Id, ct) });
});

app.MapPatch("/v1/variations/{variationId:long}", async (long variationId, HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    if (!await db.TokenValidAsync(request, ct)) return Results.Unauthorized();
    var body = await request.ReadFromJsonAsync<StockChange>(cancellationToken: ct);
    if (body is null || body.Stock < 0) return Results.BadRequest(new { error = "invalid_stock" });
    return await db.ChangeVariantStockAsync(variationId, body.Stock, ct) ? Results.Ok(new { success = true }) : Results.NotFound();
});

app.MapPatch("/v1/products/{productId:long}", async (long productId, HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    if (!await db.TokenValidAsync(request, ct)) return Results.Unauthorized();
    var body = await request.ReadFromJsonAsync<StockChange>(cancellationToken: ct);
    if (body is null || body.Stock < 0) return Results.BadRequest(new { error = "invalid_stock" });
    return await db.ChangeStockAsync(productId, body.Stock, ct) ? Results.Ok(new { success = true }) : Results.NotFound();
});

app.MapMethods("/v3/products/{productId:long}/stock", new[] { "PATCH", "POST" }, async (long productId, HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    if (!await db.TokenValidAsync(request, ct)) return Results.Unauthorized();
    var body = await request.ReadFromJsonAsync<StockChange>(cancellationToken: ct);
    if (body is null || body.Stock < 0) return Results.BadRequest(new { error = "invalid_stock" });
    var changed = await db.ChangeStockAsync(productId, body.Stock, ct);
    return changed ? Results.Ok(new { success = true }) : Results.NotFound();
});

app.MapGet("/sandbox/vendor", async (SqlServerDemoStore db, CancellationToken ct) =>
    Results.Content(Html.Vendor(await db.ListVendorsAsync(ct), await db.ListProductsAsync(1001, ct)), "text/html; charset=utf-8"));

app.MapPost("/sandbox/vendor/products/{productId:long}/stock", async (long productId, HttpRequest request, SqlServerDemoStore db, CancellationToken ct) =>
{
    var form = await request.ReadFormAsync(ct);
    if (int.TryParse(form["stock"], out var stock) && stock >= 0) await db.ChangeStockAsync(productId, stock, ct);
    return Results.Redirect("/sandbox/vendor");
});

app.MapPost("/sandbox/reset", async (SqlServerDemoStore db, CancellationToken ct) => { await db.ResetAsync(ct); return Results.Redirect("/sandbox/vendor"); });

app.Run();

record StockChange(int Stock);
