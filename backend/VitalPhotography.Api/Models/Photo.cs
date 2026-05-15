namespace VitalPhotography.Api.Models;

public class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;   // "landscape-nature" | "street-urban"
    public string Description { get; set; } = string.Empty;
    public DateOnly ShootDate { get; set; }
    public bool Visible { get; set; } = true;
    public int Order { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string DisplayUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
