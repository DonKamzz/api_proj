using System.ComponentModel.DataAnnotations;

namespace api_proj.DTOs;

public class EventCreateRequest
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    [Required, MaxLength(150)]
    public string Location { get; set; } = string.Empty;

    [Required, Range(0.01, 10000)]
    public decimal DistanceKm { get; set; }

    /// <summary>Must be "Run", "Walk", or "Cycle".</summary>
    [Required]
    public string EventType { get; set; } = string.Empty;

    public string? BannerImageUrl { get; set; }
}

public class EventUpdateRequest : EventCreateRequest
{
}

public class EventResponse
{
    public int EventId { get; set; }
    public int OrganiserId { get; set; }
    public string OrganiserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? BannerImageUrl { get; set; }
    public List<CategoryResponse> Categories { get; set; } = new();
    public List<RouteResponse> Routes { get; set; } = new();
}
