using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace VitalPhotography.Api.Services;

public class AzureBlobStorageService(IConfiguration config) : IStorageService
{
    private readonly BlobServiceClient _client =
        new(config["AzureBlob:ConnectionString"]);

    private readonly string _container =
        config["AzureBlob:ContainerName"] ?? "photos";

    private readonly string _cdnBase =
        config["AzureBlob:CdnBaseUrl"]?.TrimEnd('/') ?? string.Empty;

    public async Task<string> SaveAsync(Stream data, string fileName, string folder, CancellationToken ct = default)
    {
        var container = _client.GetBlobContainerClient(_container);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var blobName = $"{folder}/{fileName}";
        var blob = container.GetBlobClient(blobName);

        var headers = new BlobHttpHeaders
        {
            ContentType    = "image/webp",
            CacheControl   = "public, max-age=31536000, immutable",
        };

        data.Position = 0;
        await blob.UploadAsync(data, new BlobUploadOptions { HttpHeaders = headers }, ct);

        // Prefer CDN URL if configured, otherwise raw blob URL
        return string.IsNullOrEmpty(_cdnBase)
            ? blob.Uri.ToString()
            : $"{_cdnBase}/{blobName}";
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var container = _client.GetBlobContainerClient(_container);

        // Extract blob name from URL regardless of CDN vs raw blob origin
        var uri      = new Uri(url);
        var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
        var blobName = segments.Length == 2 ? segments[1] : uri.AbsolutePath.TrimStart('/');

        await container.GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: ct);
    }
}
