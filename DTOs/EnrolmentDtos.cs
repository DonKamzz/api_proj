using System.ComponentModel.DataAnnotations;

namespace api_proj.DTOs;

public class EnrolRequest
{
    [Required]
    public int CategoryId { get; set; }
}

public class EnrolmentResponse
{
    public int EnrolmentId { get; set; }
    public int ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string EnrolmentStatus { get; set; } = string.Empty;
    public DateTime EnrolmentDate { get; set; }
    public ResultResponse? Result { get; set; }
}
