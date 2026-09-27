using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface ICoreService
{
    Task<JsonElement> GetCurrentUserAsync(CancellationToken ct = default);
    Task<JsonElement> GetProductsAsync(string? query = null, CancellationToken ct = default);
    Task<JsonElement> GetCategoriesAsync(CancellationToken ct = default);
    Task<JsonElement> GetCategoryAsync(int categoryId, CancellationToken ct = default);
    Task<JsonElement> GetCategoryAttributesAsync(int categoryId, int? productId = null, int? vendorId = null, bool excludeMultiSelects = true, CancellationToken ct = default);
    Task<JsonElement> CreateProductsBulkActionRequestAsync(int vendorId, object request, CancellationToken ct = default);
    Task<JsonElement> GetProductsBulkActionRequestsAsync(int vendorId, int page = 1, int perPage = 10, CancellationToken ct = default);
    Task<JsonElement> GetProductsBulkActionRequestsCountAsync(int vendorId, CancellationToken ct = default);
    Task<JsonElement> GetProductsUnsuccessfulBulkActionRequestsAsync(int requestId, int page = 1, int perPage = 10, CancellationToken ct = default);
    Task<JsonElement> GetProductShelvesAsync(int productId, CancellationToken ct = default);
    Task<JsonElement> CreateDiscountAsync(int vendorId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteDiscountAsync(int vendorId, object request, CancellationToken ct = default);
    Task<JsonElement> CreateUserMobileConfirmationRequestAsync(int userId, CancellationToken ct = default);
    Task<JsonElement> VerifyUserMobileConfirmationRequestAsync(int userId, object request, CancellationToken ct = default);
    Task<JsonElement> CreateUserMobileChangeRequestAsync(int userId, object request, CancellationToken ct = default);
    Task<JsonElement> VerifyUserMobileChangeRequestAsync(int userId, object request, CancellationToken ct = default);
    Task<JsonElement> GetUserBankAccountsAsync(int userId, CancellationToken ct = default);
    Task<JsonElement> CreateUserBankAccountAsync(int userId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteUserBankAccountAsync(int userId, int bankAccountId, CancellationToken ct = default);
    Task<JsonElement> UpdateUserBankAccountAsync(int userId, int bankAccountId, object request, CancellationToken ct = default);
    Task<JsonElement> VerifyUserBankAccountOtpAsync(int userId, object request, CancellationToken ct = default);
    Task<JsonElement> VerifyUserBankAccountAsync(int userId, object request, CancellationToken ct = default);
    Task<JsonElement> UpdateUserVerificationAsync(int userId, object request, CancellationToken ct = default);
    Task<JsonElement> CreateShelveAsync(object request, CancellationToken ct = default);
    Task<JsonElement> UpdateShelveAsync(int shelveId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteShelveAsync(int shelveId, CancellationToken ct = default);
    Task<JsonElement> GetShelveProductsAsync(int shelveId, string? title = null, CancellationToken ct = default);
    Task<JsonElement> UpdateShelveProductsAsync(int shelveId, object request, CancellationToken ct = default);
    Task<JsonElement> DeleteShelveProductAsync(int shelveId, int productId, CancellationToken ct = default);
}

public sealed class CoreService(IBasalamHttpClient client) : ICoreService
{
    public Task<JsonElement> GetCurrentUserAsync(CancellationToken ct = default) => Get("/v1/users/me", ct);
    public Task<JsonElement> GetProductsAsync(string? query = null, CancellationToken ct = default) => Get("/v1/products" + Suffix(query), ct);
    public Task<JsonElement> GetCategoriesAsync(CancellationToken ct = default) => Get("/v1/categories", ct);
    public Task<JsonElement> GetCategoryAsync(int categoryId, CancellationToken ct = default) => Get($"/v1/categories/{Id(categoryId, "categoryId")}", ct);
    public Task<JsonElement> GetCategoryAttributesAsync(int categoryId, int? productId = null, int? vendorId = null, bool excludeMultiSelects = true, CancellationToken ct = default) =>
        Get($"/v1/categories/{Id(categoryId, "categoryId")}/attributes" + Query(("product_id", productId), ("vendor_id", vendorId), ("exclude_multi_selects", excludeMultiSelects)), ct);

    public Task<JsonElement> CreateProductsBulkActionRequestAsync(int vendorId, object request, CancellationToken ct = default) => Post($"/v1/vendors/{Id(vendorId, "vendorId")}/batch-jobs", request, ct);
    public Task<JsonElement> GetProductsBulkActionRequestsAsync(int vendorId, int page = 1, int perPage = 10, CancellationToken ct = default) =>
        Get($"/v1/vendors/{Id(vendorId, "vendorId")}/batch-jobs" + Page(page, perPage), ct);
    public Task<JsonElement> GetProductsBulkActionRequestsCountAsync(int vendorId, CancellationToken ct = default) => Get($"/v1/vendors/{Id(vendorId, "vendorId")}/batch-jobs/count", ct);
    public Task<JsonElement> GetProductsUnsuccessfulBulkActionRequestsAsync(int requestId, int page = 1, int perPage = 10, CancellationToken ct = default) =>
        Get($"/v1/batch-jobs/{Id(requestId, "requestId")}/failed-items" + Page(page, perPage), ct);
    public Task<JsonElement> GetProductShelvesAsync(int productId, CancellationToken ct = default) => Get($"/v1/products/{Id(productId, "productId")}/shelves", ct);
    public Task<JsonElement> CreateDiscountAsync(int vendorId, object request, CancellationToken ct = default) => Post($"/v1/vendors/{Id(vendorId, "vendorId")}/discounts", request, ct);
    public Task<JsonElement> DeleteDiscountAsync(int vendorId, object request, CancellationToken ct = default) => Delete($"/v1/vendors/{Id(vendorId, "vendorId")}/discounts", request, ct);

    public Task<JsonElement> CreateUserMobileConfirmationRequestAsync(int userId, CancellationToken ct = default) => Post($"/v1/users/{Id(userId, "userId")}/mobile-verification-requests", new { }, ct);
    public Task<JsonElement> VerifyUserMobileConfirmationRequestAsync(int userId, object request, CancellationToken ct = default) => Post($"/v1/users/{Id(userId, "userId")}/mobile-verification-confirmations", request, ct);
    public Task<JsonElement> CreateUserMobileChangeRequestAsync(int userId, object request, CancellationToken ct = default) => Post($"/v1/users/{Id(userId, "userId")}/mobile-change-requests", request, ct);
    public Task<JsonElement> VerifyUserMobileChangeRequestAsync(int userId, object request, CancellationToken ct = default) => Post($"/v1/users/{Id(userId, "userId")}/mobile-change-confirmations", request, ct);
    public Task<JsonElement> GetUserBankAccountsAsync(int userId, CancellationToken ct = default) => Get($"/v1/users/{Id(userId, "userId")}/bank-accounts", ct);
    public Task<JsonElement> CreateUserBankAccountAsync(int userId, object request, CancellationToken ct = default) => Post($"/v1/users/{Id(userId, "userId")}/bank-accounts", request, ct);
    public Task<JsonElement> DeleteUserBankAccountAsync(int userId, int bankAccountId, CancellationToken ct = default) => Delete($"/v1/users/{Id(userId, "userId")}/bank-accounts/{Id(bankAccountId, "bankAccountId")}", ct);
    public Task<JsonElement> UpdateUserBankAccountAsync(int userId, int bankAccountId, object request, CancellationToken ct = default) => Patch($"/v1/users/{Id(userId, "userId")}/bank-accounts/{Id(bankAccountId, "bankAccountId")}", request, ct);
    public Task<JsonElement> VerifyUserBankAccountOtpAsync(int userId, object request, CancellationToken ct = default) => Post($"/v1/users/{Id(userId, "userId")}/bank-accounts/verify-otp", request, ct);
    public Task<JsonElement> VerifyUserBankAccountAsync(int userId, object request, CancellationToken ct = default) => Post($"/v1/users/{Id(userId, "userId")}/bank-accounts/verify", request, ct);
    public Task<JsonElement> UpdateUserVerificationAsync(int userId, object request, CancellationToken ct = default) => Patch($"/v1/users/{Id(userId, "userId")}/verification-requests", request, ct);

    public Task<JsonElement> CreateShelveAsync(object request, CancellationToken ct = default) => Post("/v1/shelves", request, ct);
    public Task<JsonElement> UpdateShelveAsync(int shelveId, object request, CancellationToken ct = default) => Put($"/v1/shelves/{Id(shelveId, "shelveId")}", request, ct);
    public Task<JsonElement> DeleteShelveAsync(int shelveId, CancellationToken ct = default) => Delete($"/v1/shelves/{Id(shelveId, "shelveId")}", ct);
    public Task<JsonElement> GetShelveProductsAsync(int shelveId, string? title = null, CancellationToken ct = default) => Get($"/v1/shelves/{Id(shelveId, "shelveId")}/products" + Query(("title", title)), ct);
    public Task<JsonElement> UpdateShelveProductsAsync(int shelveId, object request, CancellationToken ct = default) => Put($"/v1/shelves/{Id(shelveId, "shelveId")}/products", request, ct);
    public Task<JsonElement> DeleteShelveProductAsync(int shelveId, int productId, CancellationToken ct = default) => Delete($"/v1/shelves/{Id(shelveId, "shelveId")}/products/{Id(productId, "productId")}", ct);

    private Task<JsonElement> Get(string path, CancellationToken ct) => client.GetAsync<JsonElement>(path, ct);
    private Task<JsonElement> Post(string path, object request, CancellationToken ct) { ArgumentNullException.ThrowIfNull(request); return client.PostAsync<JsonElement>(path, request, ct); }
    private Task<JsonElement> Put(string path, object request, CancellationToken ct) { ArgumentNullException.ThrowIfNull(request); return client.PutAsync<JsonElement>(path, request, ct); }
    private Task<JsonElement> Patch(string path, object request, CancellationToken ct) { ArgumentNullException.ThrowIfNull(request); return client.PatchAsync<JsonElement>(path, request, ct); }
    private Task<JsonElement> Delete(string path, CancellationToken ct) => client.DeleteAsync<JsonElement>(path, ct);
    private Task<JsonElement> Delete(string path, object request, CancellationToken ct) { ArgumentNullException.ThrowIfNull(request); return client.DeleteAsync<JsonElement>(path, request, ct); }
    private static int Id(int value, string field) { if (value <= 0) throw Invalid(field); return value; }
    private static string Page(int page, int perPage) { if (page < 1 || perPage is < 1 or > 100) throw Invalid("pagination"); return $"?page={page}&per_page={perPage}"; }
    private static string Suffix(string? query) => string.IsNullOrWhiteSpace(query) ? string.Empty : query.StartsWith('?') ? query : "?" + query;
    private static string Query(params (string Name, object? Value)[] values)
    {
        var parts = values.Where(x => x.Value is not null).Select(x => Uri.EscapeDataString(x.Name) + "=" + Uri.EscapeDataString(x.Value!.ToString()!));
        var query = string.Join('&', parts); return query.Length == 0 ? string.Empty : "?" + query;
    }
    private static BasalamValidationError Invalid(string field) => new(new Dictionary<string, IReadOnlyList<string>> { [field] = ["Value is invalid"] });
}
