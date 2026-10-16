using System.ComponentModel.DataAnnotations;

namespace api_proj.DTOs;

public class CategoryRequest
{
    [Required, MaxLength(50)]
    public string CategoryName { get; set; } = string.Empty;

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public decimal? DistanceKm { get; set; }
}

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public int EventId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public decimal? DistanceKm { get; set; }
}

public class RouteResponse
{
    public int RouteId { get; set; }
    public int EventId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public int? ElevationGainM { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
    public string? MapUrl { get; set; }
}
