using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace VitalPhotography.Api.Services;

public class ImageService
{
    private static readonly WebpEncoder Encoder = new() { Quality = 85 };

    public async Task<(Stream Thumbnail, Stream Display)> ResizeAsync(Stream source, CancellationToken ct = default)
    {
        source.Position = 0;
        using var image = await Image.LoadAsync(source, ct);

        var thumb   = new MemoryStream();
        var display = new MemoryStream();

        using (var clone = image.Clone(x => x.Resize(new ResizeOptions
        {
            Size = new Size(400, 0),
            Mode = ResizeMode.Max,
        })))
            await clone.SaveAsync(thumb, Encoder, ct);

        using (var clone = image.Clone(x => x.Resize(new ResizeOptions
        {
            Size = new Size(1800, 0),
            Mode = ResizeMode.Max,
        })))
            await clone.SaveAsync(display, Encoder, ct);

        thumb.Position   = 0;
        display.Position = 0;
        return (thumb, display);
    }
}
