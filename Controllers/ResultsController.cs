using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_proj.Auth;
using api_proj.Data;
using api_proj.DTOs;
using api_proj.Models;

namespace api_proj.Controllers;

[ApiController]
public class ResultsController : ControllerBase
{
    private readonly ApplicationDBContext _db;

    public ResultsController(ApplicationDBContext db)
    {
        _db = db;
    }

    /// <summary>Captures/publishes a finish time and position for an enrolled Participant after the event.</summary>
    [HttpPost("api/events/{eventId:int}/results")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<ActionResult<ResultResponse>> CreateResult(int eventId, ResultCreateRequest request)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound(new { message = "Event does not exist." });

        if (ev.OrganiserId != HttpContext.GetUserId()) return StatusCode(403, new { message = "You do not have permission to do this." });

        var enrolment = await _db.Enrolments
            .Include(en => en.Category)
            .FirstOrDefaultAsync(en => en.EnrolmentId == request.EnrolmentId && en.EventId == eventId);

        if (enrolment is null)
        {
            return NotFound(new { message = "Enrolment does not exist for this event." });
        }

        if (request.FinishPosition > request.TotalFinishers)
        {
            return BadRequest(new { message = "FinishPosition cannot be greater than TotalFinishers." });
        }

        var alreadyHasResult = await _db.Results.AnyAsync(r => r.EnrolmentId == request.EnrolmentId);
        if (alreadyHasResult)
        {
            return Conflict(new { message = "A result has already been captured for this enrolment. Use PUT to correct it." });
        }

        var result = new Result
        {
            EnrolmentId = request.EnrolmentId,
            FinishTimeSeconds = request.FinishTimeSeconds,
            FinishPosition = request.FinishPosition,
            TotalFinishers = request.TotalFinishers,
            PublishedAt = DateTime.UtcNow
        };

        _db.Results.Add(result);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetResult), new { id = result.ResultId }, ToResultResponse(result, ev, enrolment.Category!));
    }

    /// <summary>Corrects a previously captured result (owner Organiser only).</summary>
    [HttpPut("api/results/{id:int}")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<ActionResult<ResultResponse>> UpdateResult(int id, ResultUpdateRequest request)
    {
        var result = await _db.Results
            .Include(r => r.Enrolment).ThenInclude(en => en!.Event)
            .Include(r => r.Enrolment).ThenInclude(en => en!.Category)
            .FirstOrDefaultAsync(r => r.ResultId == id);

        if (result is null) return NotFound();

        var ownerId = result.Enrolment!.Event!.OrganiserId;
        if (ownerId != HttpContext.GetUserId()) return StatusCode(403, new { message = "You do not have permission to do this." });

        if (request.FinishPosition > request.TotalFinishers)
        {
            return BadRequest(new { message = "FinishPosition cannot be greater than TotalFinishers." });
        }

        result.FinishTimeSeconds = request.FinishTimeSeconds;
        result.FinishPosition = request.FinishPosition;
        result.TotalFinishers = request.TotalFinishers;

        await _db.SaveChangesAsync();
        return Ok(ToResultResponse(result, result.Enrolment.Event, result.Enrolment.Category!));
    }

    /// <summary>Returns one result (viewable by the owning Participant or the Organiser of that event).</summary>
    [HttpGet("api/results/{id:int}")]
    [Authorize]
    public async Task<ActionResult<ResultResponse>> GetResult(int id)
    {
        var result = await _db.Results
            .Include(r => r.Enrolment).ThenInclude(en => en!.Event)
            .Include(r => r.Enrolment).ThenInclude(en => en!.Category)
            .FirstOrDefaultAsync(r => r.ResultId == id);

        if (result is null) return NotFound();

        var currentUserId = HttpContext.GetUserId();
        var isOwningParticipant = result.Enrolment!.ParticipantId == currentUserId;
        var isOwningOrganiser = result.Enrolment.Event!.OrganiserId == currentUserId;

        if (!isOwningParticipant && !isOwningOrganiser) return StatusCode(403, new { message = "You do not have permission to do this." });

        return Ok(ToResultResponse(result, result.Enrolment.Event, result.Enrolment.Category!));
    }

    private static ResultResponse ToResultResponse(Result r, Event ev, Category category) => new()
    {
        ResultId = r.ResultId,
        EnrolmentId = r.EnrolmentId,
        EventName = ev.Name,
        EventDate = ev.EventDate,
        CategoryName = category.CategoryName,
        FinishTimeSeconds = r.FinishTimeSeconds,
        FinishPosition = r.FinishPosition,
        TotalFinishers = r.TotalFinishers,
        PublishedAt = r.PublishedAt
    };
}
