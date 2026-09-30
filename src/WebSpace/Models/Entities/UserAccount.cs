using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: USER_ACCOUNT</summary>
public class UserAccount
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>Never plain text.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public AccountStatus AccountStatus { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    public int FailedLoginCount { get; set; }

    /// <summary>5 failed logins pause login for 15 minutes.</summary>
    public DateTime? LockoutEndAt { get; set; }

    /// <summary>Rotated on password reset.</summary>
    public string SecurityStamp { get; set; } = string.Empty;

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Role Role { get; set; } = null!;

    public ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public ICollection<CourseTutor> CourseTutors { get; set; } = new List<CourseTutor>();

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public ICollection<DiscussionPost> DiscussionPosts { get; set; } = new List<DiscussionPost>();

    public ICollection<DiscussionReply> DiscussionReplies { get; set; } = new List<DiscussionReply>();

    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();

    public ICollection<ChallengeProgress> ChallengeProgresses { get; set; } = new List<ChallengeProgress>();

    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    /// <summary>Feedback written by this user as tutor (FEEDBACK.tutor_user_id).</summary>
    public ICollection<Feedback> FeedbackGiven { get; set; } = new List<Feedback>();
}
