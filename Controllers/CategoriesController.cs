using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_proj.Auth;
using api_proj.Data;
using api_proj.DTOs;
using api_proj.Models;

namespace api_proj.Controllers;

[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDBContext _db;

    public CategoriesController(ApplicationDBContext db)
    {
        _db = db;
    }

    /// <summary>Lists the age/distance categories available for an event.</summary>
    [HttpGet("api/events/{eventId:int}/categories")]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategoriesForEvent(int eventId)
    {
        var eventExists = await _db.Events.AnyAsync(e => e.EventId == eventId);
        if (!eventExists) return NotFound(new { message = "Event does not exist." });

        var categories = await _db.Categories.Where(c => c.EventId == eventId).ToListAsync();
        return Ok(categories.Select(ToCategoryResponse));
    }

    /// <summary>Adds a category to an event (owner Organiser only).</summary>
    [HttpPost("api/events/{eventId:int}/categories")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<ActionResult<CategoryResponse>> CreateCategory(int eventId, CategoryRequest request)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound(new { message = "Event does not exist." });

        if (ev.OrganiserId != HttpContext.GetUserId()) return StatusCode(403, new { message = "You do not have permission to do this." });

        var category = new Category
        {
            EventId = eventId,
            CategoryName = request.CategoryName,
            MinAge = request.MinAge,
            MaxAge = request.MaxAge,
            DistanceKm = request.DistanceKm
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategoriesForEvent), new { eventId }, ToCategoryResponse(category));
    }

    /// <summary>Updates a category's details (owner Organiser only).</summary>
    [HttpPut("api/categories/{id:int}")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
    {
        var category = await _db.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryId == id);
        if (category is null) return NotFound();

        if (category.Event!.OrganiserId != HttpContext.GetUserId()) return StatusCode(403, new { message = "You do not have permission to do this." });

        category.CategoryName = request.CategoryName;
        category.MinAge = request.MinAge;
        category.MaxAge = request.MaxAge;
        category.DistanceKm = request.DistanceKm;

        await _db.SaveChangesAsync();
        return Ok(ToCategoryResponse(category));
    }

    /// <summary>Removes a category from an event (owner Organiser only).</summary>
    [HttpDelete("api/categories/{id:int}")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _db.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryId == id);
        if (category is null) return NotFound();

        if (category.Event!.OrganiserId != HttpContext.GetUserId()) return StatusCode(403, new { message = "You do not have permission to do this." });

        var hasActiveEnrolments = await _db.Enrolments.AnyAsync(
            en => en.CategoryId == id && en.EnrolmentStatus != EnrolmentStatuses.Cancelled);

        if (hasActiveEnrolments)
        {
            return Conflict(new { message = "This category has active enrolments and cannot be deleted." });
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static CategoryResponse ToCategoryResponse(Category c) => new()
    {
        CategoryId = c.CategoryId,
        EventId = c.EventId,
        CategoryName = c.CategoryName,
        MinAge = c.MinAge,
        MaxAge = c.MaxAge,
        DistanceKm = c.DistanceKm
    };
}
