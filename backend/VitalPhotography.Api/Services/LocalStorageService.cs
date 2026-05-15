namespace VitalPhotography.Api.Services;

public class LocalStorageService(IWebHostEnvironment env, IHttpContextAccessor http) : IStorageService
{
    public async Task<string> SaveAsync(Stream data, string fileName, string folder, CancellationToken ct = default)
    {
        var dir = Path.Combine(env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(dir);

        var path = Path.Combine(dir, fileName);
        await using var fs = File.Create(path);
        await data.CopyToAsync(fs, ct);

        var req = http.HttpContext!.Request;
        return $"{req.Scheme}://{req.Host}/uploads/{folder}/{fileName}";
    }

    public Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var uri = new Uri(url);
        var relative = uri.AbsolutePath.TrimStart('/');
        var path = Path.Combine(env.WebRootPath, relative.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}
