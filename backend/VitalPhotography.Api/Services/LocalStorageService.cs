namespace VitalPhotography.Api.Services;

public class LocalStorageService(IConfiguration config, IHttpContextAccessor http) : IStorageService
{
    // In production the Render persistent disk is mounted at /data.
    // In dev, falls back to wwwroot/uploads (relative to the app).
    private string UploadsRoot =>
        config["Storage:Root"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public async Task<string> SaveAsync(Stream data, string fileName, string folder, CancellationToken ct = default)
    {
        var dir = Path.Combine(UploadsRoot, folder);
        Directory.CreateDirectory(dir);

        var path = Path.Combine(dir, fileName);
        await using var fs = File.Create(path);
        await data.CopyToAsync(fs, ct);

        var req = http.HttpContext!.Request;
        return $"{req.Scheme}://{req.Host}/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var uri  = new Uri(url);
        var rel  = uri.AbsolutePath.TrimStart('/').Substring("uploads/".Length);
        var path = Path.Combine(UploadsRoot, rel.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}
