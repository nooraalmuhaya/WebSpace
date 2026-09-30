namespace WebSpace.Models.Entities;

/// <summary>ERD: CHALLENGE_PROGRESS</summary>
public class ChallengeProgress
{
    public int ChallengeProgressId { get; set; }

    public int StudentUserId { get; set; }

    public int ChallengeId { get; set; }

    /// <summary>Auto-saved working copy.</summary>
    public string SavedCode { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime LastSavedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public UserAccount Student { get; set; } = null!;

    public CodingChallenge Challenge { get; set; } = null!;
}
