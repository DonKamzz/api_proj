using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_proj.Auth;
using api_proj.Data;
using api_proj.DTOs;
using api_proj.Models;

namespace api_proj.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private static readonly string[] ValidEventTypes = { EventTypes.Run, EventTypes.Walk, EventTypes.Cycle };

    private readonly ApplicationDBContext _db;

    public EventsController(ApplicationDBContext db)
    {
        _db = db;
    }

    /// <summary>Lists all events, with optional filters.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetEvents(
        [FromQuery] string? type, [FromQuery] string? location, [FromQuery] DateTime? fromDate)
    {
        var query = _db.Events.Include(e => e.Organiser).AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(e => e.EventType == type);

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(e => e.Location.Contains(location));

        if (fromDate.HasValue)
            query = query.Where(e => e.EventDate >= fromDate.Value);

        var events = await query.OrderBy(e => e.EventDate).ToListAsync();
        return Ok(events.Select(e => ToEventResponse(e, includeChildren: false)));
    }

    /// <summary>Returns full details of one event, including its categories and routes.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventResponse>> GetEvent(int id)
    {
        var ev = await _db.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories)
            .Include(e => e.Routes)
            .FirstOrDefaultAsync(e => e.EventId == id);

        if (ev is null) return NotFound();

        return Ok(ToEventResponse(ev, includeChildren: true));
    }

    /// <summary>Creates a new event owned by the logged-in Organiser.</summary>
    [HttpPost]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<ActionResult<EventResponse>> CreateEvent(EventCreateRequest request)
    {
        if (!ValidEventTypes.Contains(request.EventType))
        {
            return BadRequest(new { message = "EventType must be 'Run', 'Walk', or 'Cycle'." });
        }

        var organiserId = HttpContext.GetUserId()!.Value;

        var ev = new Event
        {
            OrganiserId = organiserId,
            Name = request.Name,
            Description = request.Description,
            EventDate = request.EventDate,
            Location = request.Location,
            DistanceKm = request.DistanceKm,
            EventType = request.EventType,
            BannerImageUrl = request.BannerImageUrl,
            CreatedAt = DateTime.UtcNow
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        await _db.Entry(ev).Reference(e => e.Organiser).LoadAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = ev.EventId }, ToEventResponse(ev, includeChildren: true));
    }

    /// <summary>Updates an event (owner Organiser only).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<ActionResult<EventResponse>> UpdateEvent(int id, EventUpdateRequest request)
    {
        if (!ValidEventTypes.Contains(request.EventType))
        {
            return BadRequest(new { message = "EventType must be 'Run', 'Walk', or 'Cycle'." });
        }

        var ev = await _db.Events
            .Include(e => e.Organiser)
            .Include(e => e.Categories)
            .Include(e => e.Routes)
            .FirstOrDefaultAsync(e => e.EventId == id);

        if (ev is null) return NotFound();

        if (ev.OrganiserId != HttpContext.GetUserId())
        {
            return StatusCode(403, new { message = "You do not have permission to do this." });
        }

        ev.Name = request.Name;
        ev.Description = request.Description;
        ev.EventDate = request.EventDate;
        ev.Location = request.Location;
        ev.DistanceKm = request.DistanceKm;
        ev.EventType = request.EventType;
        ev.BannerImageUrl = request.BannerImageUrl;

        await _db.SaveChangesAsync();
        return Ok(ToEventResponse(ev, includeChildren: true));
    }

    /// <summary>Deletes an event (owner Organiser only).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return NotFound();

        if (ev.OrganiserId != HttpContext.GetUserId())
        {
            return StatusCode(403, new { message = "You do not have permission to do this." });
        }

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static EventResponse ToEventResponse(Event ev, bool includeChildren) => new()
    {
        EventId = ev.EventId,
        OrganiserId = ev.OrganiserId,
        OrganiserName = ev.Organiser is null ? string.Empty : $"{ev.Organiser.FirstName} {ev.Organiser.LastName}",
        Name = ev.Name,
        Description = ev.Description,
        EventDate = ev.EventDate,
        Location = ev.Location,
        DistanceKm = ev.DistanceKm,
        EventType = ev.EventType,
        BannerImageUrl = ev.BannerImageUrl,
        Categories = !includeChildren ? new() : ev.Categories.Select(c => new CategoryResponse
        {
            CategoryId = c.CategoryId,
            EventId = c.EventId,
            CategoryName = c.CategoryName,
            MinAge = c.MinAge,
            MaxAge = c.MaxAge,
            DistanceKm = c.DistanceKm
        }).ToList(),
        Routes = !includeChildren ? new() : ev.Routes.Select(r => new RouteResponse
        {
            RouteId = r.RouteId,
            EventId = r.EventId,
            RouteName = r.RouteName,
            DistanceKm = r.DistanceKm,
            ElevationGainM = r.ElevationGainM,
            StartPoint = r.StartPoint,
            EndPoint = r.EndPoint,
            MapUrl = r.MapUrl
        }).ToList()
    };
}
