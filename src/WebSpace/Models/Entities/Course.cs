using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: COURSE</summary>
public class Course
{
    public int CourseId { get; set; }

    public int PrimaryRoadmapId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DifficultyLevel Difficulty { get; set; }

    public int? EstimatedWeeks { get; set; }

    public string? ThumbnailPath { get; set; }

    /// <summary>False means Draft.</summary>
    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Roadmap PrimaryRoadmap { get; set; } = null!;

    public ICollection<CourseRoadmap> CourseRoadmaps { get; set; } = new List<CourseRoadmap>();

    public ICollection<CourseLearningOutcome> LearningOutcomes { get; set; } = new List<CourseLearningOutcome>();

    public ICollection<CourseWeek> Weeks { get; set; } = new List<CourseWeek>();

    public ICollection<CourseTutor> CourseTutors { get; set; } = new List<CourseTutor>();

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();

    public ICollection<DiscussionPost> DiscussionPosts { get; set; } = new List<DiscussionPost>();
}
