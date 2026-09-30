using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: SUBMISSION. Displayed as SUB-00482.</summary>
public class Submission
{
    public int SubmissionId { get; set; }

    public int AssignmentId { get; set; }

    public int StudentUserId { get; set; }

    /// <summary>1, 2, ... on resubmission.</summary>
    public int AttemptNumber { get; set; }

    /// <summary>Optional reflection text.</summary>
    public string? SubmissionText { get; set; }

    public SubmissionStatus Status { get; set; }

    public DateTime SubmittedAt { get; set; }

    public Assignment Assignment { get; set; } = null!;

    public UserAccount Student { get; set; } = null!;

    public ICollection<SubmissionFile> Files { get; set; } = new List<SubmissionFile>();

    /// <summary>At most one review per submission.</summary>
    public Feedback? Feedback { get; set; }
}
