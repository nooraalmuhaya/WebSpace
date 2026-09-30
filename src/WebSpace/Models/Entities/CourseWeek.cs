namespace WebSpace.Models.Entities;

/// <summary>ERD: COURSE_WEEK</summary>
public class CourseWeek
{
    public int CourseWeekId { get; set; }

    public int CourseId { get; set; }

    /// <summary>Unique per course.</summary>
    public int WeekNumber { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Course Course { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public ICollection<DiscussionPost> DiscussionPosts { get; set; } = new List<DiscussionPost>();

    /// <summary>At most one quiz per week.</summary>
    public Quiz? Quiz { get; set; }

    /// <summary>At most one assignment per week.</summary>
    public Assignment? Assignment { get; set; }
}
