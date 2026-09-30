namespace WebSpace.Models.Entities;

/// <summary>ERD: QUIZ_ATTEMPT_ANSWER</summary>
public class QuizAttemptAnswer
{
    public int AnswerId { get; set; }

    public int AttemptId { get; set; }

    public int QuestionId { get; set; }

    /// <summary>Null means the question was not answered.</summary>
    public int? SelectedOptionId { get; set; }

    /// <summary>Set on submit.</summary>
    public bool IsCorrect { get; set; }

    /// <summary>Set on submit.</summary>
    public int MarksAwarded { get; set; }

    public QuizAttempt Attempt { get; set; } = null!;

    public QuizQuestion Question { get; set; } = null!;

    public QuizOption? SelectedOption { get; set; }
}
