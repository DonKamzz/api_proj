using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_proj.Models;

public static class EnrolmentStatuses
{
    public const string Pending = "Pending";
    public const string Confirmed = "Confirmed";
    public const string Cancelled = "Cancelled";
}

public class Enrolment
{
    [Key]
    public int EnrolmentId { get; set; }

    [Required]
    public int ParticipantId { get; set; }

    [Required]
    public int EventId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required, MaxLength(20)]
    public string EnrolmentStatus { get; set; } = EnrolmentStatuses.Pending;

    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(ParticipantId))]
    public User? Participant { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    public Result? Result { get; set; }
}
