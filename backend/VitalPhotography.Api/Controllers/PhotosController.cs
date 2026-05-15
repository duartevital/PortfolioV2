using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitalPhotography.Api.Data;
using VitalPhotography.Api.Models;

namespace VitalPhotography.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PhotosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var query = db.Photos
            .Where(p => p.Visible)
            .OrderBy(p => p.Order)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category == category);

        return Ok(await query.ToListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var photo = await db.Photos.FindAsync(id);
        return photo is null ? NotFound() : Ok(photo);
    }
}
