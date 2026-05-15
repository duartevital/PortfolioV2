using Amazon.S3;
using Amazon.S3.Model;

namespace VitalPhotography.Api.Services;

public class CloudflareR2StorageService(IConfiguration config) : IStorageService
{
    private readonly IAmazonS3 _s3 = new AmazonS3Client(
        config["CloudflareR2:AccessKeyId"],
        config["CloudflareR2:SecretAccessKey"],
        new AmazonS3Config
        {
            ServiceURL   = $"https://{config["CloudflareR2:AccountId"]}.r2.cloudflarestorage.com",
            ForcePathStyle = true,
        });

    private readonly string _bucket    = config["CloudflareR2:BucketName"] ?? "photos";
    private readonly string _publicUrl = config["CloudflareR2:PublicUrl"]?.TrimEnd('/') ?? string.Empty;

    public async Task<string> SaveAsync(Stream data, string fileName, string folder, CancellationToken ct = default)
    {
        var key = $"{folder}/{fileName}";
        data.Position = 0;

        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName  = _bucket,
            Key         = key,
            InputStream = data,
            ContentType = "image/webp",
            Headers     = { CacheControl = "public, max-age=31536000, immutable" },
        }, ct);

        return string.IsNullOrEmpty(_publicUrl)
            ? $"https://pub-{_bucket}.r2.dev/{key}"
            : $"{_publicUrl}/{key}";
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var uri = new Uri(url);
        var key = uri.AbsolutePath.TrimStart('/');
        await _s3.DeleteObjectAsync(_bucket, key, ct);
    }
}
