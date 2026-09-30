using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: LESSON_PROGRESS</summary>
public class LessonProgress
{
    public int LessonProgressId { get; set; }

    public int StudentUserId { get; set; }

    public int LessonId { get; set; }

    public LessonProgressStatus Status { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public UserAccount Student { get; set; } = null!;

    public Lesson Lesson { get; set; } = null!;
}
