namespace WebSpace.Models.Entities;

/// <summary>ERD: QUIZ. One quiz per course week.</summary>
public class Quiz
{
    public int QuizId { get; set; }

    public int CourseWeekId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Instructions { get; set; }

    public int PassMarkPercent { get; set; } = 50;

    /// <summary>Null means unlimited attempts.</summary>
    public int? MaxAttempts { get; set; }

    public CourseWeek CourseWeek { get; set; } = null!;

    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();

    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
