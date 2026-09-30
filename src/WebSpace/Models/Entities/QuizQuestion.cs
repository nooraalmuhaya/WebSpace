namespace WebSpace.Models.Entities;

/// <summary>ERD: QUIZ_QUESTION</summary>
public class QuizQuestion
{
    public int QuestionId { get; set; }

    public int QuizId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public string? CodeSnippet { get; set; }

    public int Marks { get; set; }

    /// <summary>Unique per quiz.</summary>
    public int DisplayOrder { get; set; }

    public string? Explanation { get; set; }

    public Quiz Quiz { get; set; } = null!;

    public ICollection<QuizOption> Options { get; set; } = new List<QuizOption>();

    public ICollection<QuizAttemptAnswer> AttemptAnswers { get; set; } = new List<QuizAttemptAnswer>();
}
