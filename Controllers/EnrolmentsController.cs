using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_proj.Auth;
using api_proj.Data;
using api_proj.DTOs;
using api_proj.Models;

namespace api_proj.Controllers;

[ApiController]
public class EnrolmentsController : ControllerBase
{
    private readonly ApplicationDBContext _db;

    public EnrolmentsController(ApplicationDBContext db)
    {
        _db = db;
    }

    /// <summary>Enrols the logged-in Participant into an event under a chosen category.</summary>
    [HttpPost("api/events/{eventId:int}/enrolments")]
    [Authorize(Roles = UserRoles.Participant)]
    public async Task<ActionResult<EnrolmentResponse>> Enrol(int eventId, EnrolRequest request)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound(new { message = "Event does not exist." });

        var category = await _db.Categories.FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId && c.EventId == eventId);
        if (category is null) return NotFound(new { message = "Category does not exist for this event." });

        var participantId = HttpContext.GetUserId()!.Value;

        var alreadyEnrolled = await _db.Enrolments.AnyAsync(en => en.EventId == eventId && en.ParticipantId == participantId);
        if (alreadyEnrolled)
        {
            return Conflict(new { message = "You are already enrolled in this event." });
        }

        var enrolment = new Enrolment
        {
            ParticipantId = participantId,
            EventId = eventId,
            CategoryId = request.CategoryId,
            EnrolmentStatus = EnrolmentStatuses.Confirmed,
            EnrolmentDate = DateTime.UtcNow
        };

        _db.Enrolments.Add(enrolment);
        await _db.SaveChangesAsync();

        await _db.Entry(enrolment).Reference(en => en.Participant).LoadAsync();
        await _db.Entry(enrolment).Reference(en => en.Event).LoadAsync();
        await _db.Entry(enrolment).Reference(en => en.Category).LoadAsync();

        return CreatedAtAction(nameof(GetEnrolmentsForEvent), new { eventId }, ToEnrolmentResponse(enrolment));
    }

    /// <summary>Returns all Participants enrolled in an event the Organiser owns.</summary>
    [HttpGet("api/events/{eventId:int}/enrolments")]
    [Authorize(Roles = UserRoles.Organiser)]
    public async Task<ActionResult<IEnumerable<EnrolmentResponse>>> GetEnrolmentsForEvent(int eventId)
    {
        var ev = await _db.Events.FindAsync(eventId);
        if (ev is null) return NotFound();

        if (ev.OrganiserId != HttpContext.GetUserId()) return StatusCode(403, new { message = "You do not have permission to do this." });

        var enrolments = await _db.Enrolments
            .Include(en => en.Participant)
            .Include(en => en.Event)
            .Include(en => en.Category)
            .Include(en => en.Result)
            .Where(en => en.EventId == eventId)
            .ToListAsync();

        return Ok(enrolments.Select(ToEnrolmentResponse));
    }

    /// <summary>Cancels the logged-in Participant's own enrolment, provided the event has not started.</summary>
    [HttpDelete("api/enrolments/{id:int}")]
    [Authorize(Roles = UserRoles.Participant)]
    public async Task<IActionResult> CancelEnrolment(int id)
    {
        var enrolment = await _db.Enrolments.Include(en => en.Event).FirstOrDefaultAsync(en => en.EnrolmentId == id);
        if (enrolment is null) return NotFound();

        if (enrolment.ParticipantId != HttpContext.GetUserId()) return StatusCode(403, new { message = "You do not have permission to do this." });

        if (enrolment.Event!.EventDate <= DateTime.UtcNow)
        {
            return Conflict(new { message = "This event has already started; the enrolment can no longer be cancelled." });
        }

        _db.Enrolments.Remove(enrolment);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static EnrolmentResponse ToEnrolmentResponse(Enrolment en) => new()
    {
        EnrolmentId = en.EnrolmentId,
        ParticipantId = en.ParticipantId,
        ParticipantName = $"{en.Participant!.FirstName} {en.Participant.LastName}",
        EventId = en.EventId,
        EventName = en.Event!.Name,
        CategoryId = en.CategoryId,
        CategoryName = en.Category!.CategoryName,
        EnrolmentStatus = en.EnrolmentStatus,
        EnrolmentDate = en.EnrolmentDate,
        Result = en.Result is null ? null : new ResultResponse
        {
            ResultId = en.Result.ResultId,
            EnrolmentId = en.Result.EnrolmentId,
            EventName = en.Event.Name,
            EventDate = en.Event.EventDate,
            CategoryName = en.Category.CategoryName,
            FinishTimeSeconds = en.Result.FinishTimeSeconds,
            FinishPosition = en.Result.FinishPosition,
            TotalFinishers = en.Result.TotalFinishers,
            PublishedAt = en.Result.PublishedAt
        }
    };
}
