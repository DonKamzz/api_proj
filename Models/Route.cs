using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_proj.Models;

public class Route
{
    [Key]
    public int RouteId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required, MaxLength(100)]
    public string RouteName { get; set; } = string.Empty;

    [Required, Column(TypeName = "decimal(6,2)")]
    public decimal DistanceKm { get; set; }

    public int? ElevationGainM { get; set; }

    [MaxLength(150)]
    public string? StartPoint { get; set; }

    [MaxLength(150)]
    public string? EndPoint { get; set; }

    [MaxLength(500)]
    public string? MapUrl { get; set; }

    // Navigation
    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}
