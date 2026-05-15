namespace VitalPhotography.Api.Services;

public interface IStorageService
{
    Task<string> SaveAsync(Stream data, string fileName, string folder, CancellationToken ct = default);
    Task DeleteAsync(string url, CancellationToken ct = default);
}
