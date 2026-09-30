namespace WebSpace.Models.Entities;

/// <summary>ERD: COURSE_LEARNING_OUTCOME</summary>
public class CourseLearningOutcome
{
    public int OutcomeId { get; set; }

    public int CourseId { get; set; }

    public string OutcomeText { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public Course Course { get; set; } = null!;
}
