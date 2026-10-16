using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_proj.Auth;
using api_proj.Data;
using api_proj.DTOs;
using api_proj.Models;

namespace api_proj.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDBContext _db;

    public UsersController(ApplicationDBContext db)
    {
        _db = db;
    }

    /// <summary>Returns the logged-in user's own profile.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var user = await _db.Users.FindAsync(HttpContext.GetUserId());
        if (user is null) return NotFound();

        return Ok(ToUserResponse(user));
    }

    /// <summary>Updates the logged-in user's own profile.</summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe(UpdateProfileRequest request)
    {
        var user = await _db.Users.FindAsync(HttpContext.GetUserId());
        if (user is null) return NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.ProfilePictureUrl = request.ProfilePictureUrl;

        await _db.SaveChangesAsync();
        return Ok(ToUserResponse(user));
    }

    /// <summary>Returns all events the logged-in Participant has enrolled in, with status.</summary>
    [HttpGet("me/enrolments")]
    [Authorize(Roles = UserRoles.Participant)]
    public async Task<ActionResult<IEnumerable<EnrolmentResponse>>> GetMyEnrolments()
    {
        var participantId = HttpContext.GetUserId();

        var enrolments = await _db.Enrolments
            .Include(en => en.Event)
            .Include(en => en.Category)
            .Include(en => en.Participant)
            .Include(en => en.Result)
            .Where(en => en.ParticipantId == participantId)
            .OrderByDescending(en => en.EnrolmentDate)
            .ToListAsync();

        return Ok(enrolments.Select(ToEnrolmentResponse));
    }

    /// <summary>Returns the logged-in Participant's full personal race history.</summary>
    [HttpGet("me/results")]
    [Authorize(Roles = UserRoles.Participant)]
    public async Task<ActionResult<IEnumerable<ResultResponse>>> GetMyResults()
    {
        var participantId = HttpContext.GetUserId();

        var results = await _db.Results
            .Include(r => r.Enrolment).ThenInclude(en => en!.Event)
            .Include(r => r.Enrolment).ThenInclude(en => en!.Category)
            .Where(r => r.Enrolment!.ParticipantId == participantId)
            .OrderByDescending(r => r.PublishedAt)
            .ToListAsync();

        return Ok(results.Select(ToResultResponse));
    }

    private static UserResponse ToUserResponse(User user) => new()
    {
        UserId = user.UserId,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Role = user.Role,
        PhoneNumber = user.PhoneNumber,
        ProfilePictureUrl = user.ProfilePictureUrl
    };

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
        Result = en.Result is null ? null : ToResultResponse(en.Result)
    };

    private static ResultResponse ToResultResponse(Result r) => new()
    {
        ResultId = r.ResultId,
        EnrolmentId = r.EnrolmentId,
        EventName = r.Enrolment!.Event!.Name,
        EventDate = r.Enrolment.Event.EventDate,
        CategoryName = r.Enrolment.Category!.CategoryName,
        FinishTimeSeconds = r.FinishTimeSeconds,
        FinishPosition = r.FinishPosition,
        TotalFinishers = r.TotalFinishers,
        PublishedAt = r.PublishedAt
    };
}
