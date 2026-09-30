using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: CODING_CHALLENGE</summary>
public class CodingChallenge
{
    public int ChallengeId { get; set; }

    public int LessonId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DifficultyLevel Difficulty { get; set; }

    public string? StarterCode { get; set; }

    /// <summary>Marked "needs confirmation" in the ERD.</summary>
    public string? ExpectedOutput { get; set; }

    public Lesson Lesson { get; set; } = null!;

    public ICollection<ChallengeProgress> ChallengeProgresses { get; set; } = new List<ChallengeProgress>();
}
