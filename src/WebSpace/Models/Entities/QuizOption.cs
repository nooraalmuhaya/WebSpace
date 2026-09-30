namespace WebSpace.Models.Entities;

/// <summary>ERD: QUIZ_OPTION</summary>
public class QuizOption
{
    public int OptionId { get; set; }

    public int QuestionId { get; set; }

    public string OptionText { get; set; } = string.Empty;

    /// <summary>Exactly one option per question is correct.</summary>
    public bool IsCorrect { get; set; }

    /// <summary>Displayed as A, B, C, D.</summary>
    public int DisplayOrder { get; set; }

    public QuizQuestion Question { get; set; } = null!;

    public ICollection<QuizAttemptAnswer> SelectedInAnswers { get; set; } = new List<QuizAttemptAnswer>();
}
