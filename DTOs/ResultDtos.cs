using System.ComponentModel.DataAnnotations;

namespace api_proj.DTOs;

public class ResultCreateRequest
{
    [Required]
    public int EnrolmentId { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int FinishTimeSeconds { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int FinishPosition { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int TotalFinishers { get; set; }
}

public class ResultUpdateRequest
{
    [Required, Range(1, int.MaxValue)]
    public int FinishTimeSeconds { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int FinishPosition { get; set; }

    [Required, Range(1, int.MaxValue)]
    public int TotalFinishers { get; set; }
}

public class ResultResponse
{
    public int ResultId { get; set; }
    public int EnrolmentId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int FinishTimeSeconds { get; set; }
    public int FinishPosition { get; set; }
    public int TotalFinishers { get; set; }
    public DateTime PublishedAt { get; set; }
}
