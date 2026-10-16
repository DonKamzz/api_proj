using System.Security.Claims;

namespace api_proj.Auth;

/// <summary>
/// Small helpers for reading the logged-in user's ID and role from their
/// authentication cookie's claims (set at sign-in in AuthController).
/// [Authorize] / [Authorize(Roles = "...")] on controllers/actions already
/// handles "must be logged in" and "must have this role" - these helpers
/// are for the extra ownership checks the Build Guide's Part AB/AE/AF/AG
/// call for (e.g. confirming the logged-in Organiser owns the event
/// they're trying to edit), which [Authorize] alone can't express.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Returns the logged-in user's ID from their claims, or null if not authenticated.</summary>
    public static int? GetUserId(this HttpContext context)
    {
        var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        return claim is not null && int.TryParse(claim.Value, out var id) ? id : null;
    }

    /// <summary>Returns the logged-in user's role from their claims, or null if not authenticated.</summary>
    public static string? GetRole(this HttpContext context) =>
        context.User.FindFirst(ClaimTypes.Role)?.Value;
}
