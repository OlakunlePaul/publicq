using System.ComponentModel.DataAnnotations;

namespace PublicQ.API.Models.Requests;

public class BulkStudentEnrollmentRequest
{
    [Required]
    public List<string> StudentIds { get; set; } = new();

    [Required]
    public Guid SessionId { get; set; }

    [Required]
    public Guid TermId { get; set; }

    [Required]
    public Guid ClassLevelId { get; set; }
}
