using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_proj.Models;

public class Result
{
    [Key]
    public int ResultId { get; set; }

    [Required]
    public int EnrolmentId { get; set; }

    [Required]
    public int FinishTimeSeconds { get; set; }

    [Required]
    public int FinishPosition { get; set; }

    [Required]
    public int TotalFinishers { get; set; }

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(EnrolmentId))]
    public Enrolment? Enrolment { get; set; }
}
