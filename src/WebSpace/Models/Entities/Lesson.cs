namespace WebSpace.Models.Entities;

/// <summary>ERD: LESSON</summary>
public class Lesson
{
    public int LessonId { get; set; }

    public int CourseWeekId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    /// <summary>Unique per week.</summary>
    public int LessonOrder { get; set; }

    public bool IsRequired { get; set; } = true;

    public int? EstimatedReadMinutes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public CourseWeek CourseWeek { get; set; } = null!;

    public ICollection<LearningResource> LearningResources { get; set; } = new List<LearningResource>();

    public ICollection<CodingChallenge> CodingChallenges { get; set; } = new List<CodingChallenge>();

    public ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
}
