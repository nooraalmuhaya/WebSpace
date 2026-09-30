using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: FEEDBACK. Displayed as FB-00219.</summary>
public class Feedback
{
    public int FeedbackId { get; set; }

    public int SubmissionId { get; set; }

    /// <summary>Set to null if the tutor account is deleted.</summary>
    public int? TutorUserId { get; set; }

    /// <summary>At least 20 characters.</summary>
    public string FeedbackText { get; set; } = string.Empty;

    public FeedbackDecision Decision { get; set; }

    /// <summary>Required when Decision is NeedsImprovement.</summary>
    public string? RequiredImprovement { get; set; }

    public FeedbackStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>Null while Draft.</summary>
    public DateTime? SubmittedAt { get; set; }

    public Submission Submission { get; set; } = null!;

    public UserAccount? Tutor { get; set; }
}
