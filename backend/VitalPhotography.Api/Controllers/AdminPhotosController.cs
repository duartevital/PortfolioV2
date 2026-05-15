using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPhotography.Api.Data;
using VitalPhotography.Api.Models;
using VitalPhotography.Api.Services;

namespace VitalPhotography.Api.Controllers;

[ApiController]
[Route("api/v1/admin/photos")]
[Authorize]
public class AdminPhotosController(AppDbContext db, ImageService images, IStorageService storage) : ControllerBase
{
    // GET api/v1/admin/photos
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await db.Photos.OrderBy(p => p.Order).ToListAsync());

    // POST api/v1/admin/photos  (multipart/form-data)
    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB per file
    public async Task<IActionResult> Upload(
        [FromForm] UploadRequest req,
        CancellationToken ct)
    {
        if (req.File is null || req.File.Length == 0)
            return BadRequest(new { error = "No file provided" });

        await using var stream = req.File.OpenReadStream();
        var (thumb, display) = await images.ResizeAsync(stream, ct);

        var id = Guid.NewGuid();
        var thumbUrl   = await storage.SaveAsync(thumb,   $"{id}-thumb.webp",   "thumbnails", ct);
        var displayUrl = await storage.SaveAsync(display, $"{id}-display.webp",  "display",    ct);

        var maxOrder = await db.Photos.MaxAsync(p => (int?)p.Order) ?? -1;

        var photo = new Photo
        {
            Id           = id,
            Title        = req.Title,
            Category     = req.Category,
            Description  = req.Description ?? string.Empty,
            ShootDate    = req.ShootDate,
            Visible      = req.Visible,
            Order        = maxOrder + 1,
            ThumbnailUrl = thumbUrl,
            DisplayUrl   = displayUrl,
        };

        db.Photos.Add(photo);
        await db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = photo.Id }, photo);
    }

    // GET api/v1/admin/photos/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var photo = await db.Photos.FindAsync(id);
        return photo is null ? NotFound() : Ok(photo);
    }

    // PATCH api/v1/admin/photos/{id}
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRequest req, CancellationToken ct)
    {
        var photo = await db.Photos.FindAsync([id], ct);
        if (photo is null) return NotFound();

        if (req.Title is not null)       photo.Title       = req.Title;
        if (req.Category is not null)    photo.Category    = req.Category;
        if (req.Description is not null) photo.Description = req.Description;
        if (req.ShootDate.HasValue)      photo.ShootDate   = req.ShootDate.Value;
        if (req.Visible.HasValue)        photo.Visible     = req.Visible.Value;

        await db.SaveChangesAsync(ct);
        return Ok(photo);
    }

    // DELETE api/v1/admin/photos/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var photo = await db.Photos.FindAsync([id], ct);
        if (photo is null) return NotFound();

        await storage.DeleteAsync(photo.ThumbnailUrl, ct);
        await storage.DeleteAsync(photo.DisplayUrl,   ct);

        db.Photos.Remove(photo);
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    // PUT api/v1/admin/photos/reorder
    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequest req, CancellationToken ct)
    {
        var photos = await db.Photos.ToListAsync(ct);
        var lookup = photos.ToDictionary(p => p.Id);

        for (var i = 0; i < req.Ids.Count; i++)
        {
            if (lookup.TryGetValue(req.Ids[i], out var p))
                p.Order = i;
        }

        await db.SaveChangesAsync(ct);
        return NoContent();
    }
}

public record UploadRequest(
    IFormFile? File,
    string Title,
    string Category,
    string? Description,
    DateOnly ShootDate,
    bool Visible = true);

public record UpdateRequest(
    string? Title,
    string? Category,
    string? Description,
    DateOnly? ShootDate,
    bool? Visible);

public record ReorderRequest(List<Guid> Ids);
