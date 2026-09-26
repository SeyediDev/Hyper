using System.Net.Http.Headers;
using System.Text.Json;
using Basalam.SDK.Clients;
using Basalam.SDK.Errors;

namespace Basalam.SDK.Services;

public interface IUploadService
{
    Task<FileUploadResponse?> UploadFileAsync(Stream content, string fileName, string fileType,
        string? customUniqueName = null, int? expireMinutes = null, CancellationToken ct = default);
}

public sealed class UploadService(IBasalamHttpClient client, ILogger<UploadService>? logger = null) : IUploadService
{
    public async Task<FileUploadResponse?> UploadFileAsync(Stream content, string fileName, string fileType,
        string? customUniqueName = null, int? expireMinutes = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        if (!content.CanRead || string.IsNullOrWhiteSpace(fileName) || fileName.Length > 255
            || fileName.Any(char.IsControl) || string.IsNullOrWhiteSpace(fileType) || fileType.Length > 80)
            throw Invalid("file");
        if (expireMinutes is <= 0 or > 525600) throw Invalid("expireMinutes");

        using var form = new MultipartFormDataContent();
        var stream = new StreamContent(content);
        stream.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(stream, "file", fileName);
        form.Add(new StringContent(fileType), "file_type");
        if (!string.IsNullOrWhiteSpace(customUniqueName)) form.Add(new StringContent(customUniqueName), "custom_unique_name");
        if (expireMinutes is { } minutes) form.Add(new StringContent(minutes.ToString(System.Globalization.CultureInfo.InvariantCulture)), "expire_minutes");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/upload") { Content = form };
        logger?.LogInformation("Uploading Basalam file {FileName}", fileName);
        using var response = await client.SendAsync(request, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new BasalamAPIError($"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}", (int)response.StatusCode);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<FileUploadResponse>(json,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    private static BasalamValidationError Invalid(string field) =>
        new(new Dictionary<string, IReadOnlyList<string>> { [field] = ["Value is invalid"] });
}
