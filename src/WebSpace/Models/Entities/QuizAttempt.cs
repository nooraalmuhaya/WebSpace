using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: QUIZ_ATTEMPT</summary>
public class QuizAttempt
{
    public int AttemptId { get; set; }

    public int StudentUserId { get; set; }

    public int QuizId { get; set; }

    /// <summary>Unique per student and quiz.</summary>
    public int AttemptNumber { get; set; }

    public QuizAttemptStatus Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    /// <summary>Null until submitted.</summary>
    public int? Score { get; set; }

    /// <summary>Snapshot of the total marks when the attempt was made.</summary>
    public int MaxScore { get; set; }

    public UserAccount Student { get; set; } = null!;

    public Quiz Quiz { get; set; } = null!;

    public ICollection<QuizAttemptAnswer> Answers { get; set; } = new List<QuizAttemptAnswer>();
}
