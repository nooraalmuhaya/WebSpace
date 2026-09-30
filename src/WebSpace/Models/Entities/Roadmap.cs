using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: ROADMAP</summary>
public class Roadmap
{
    public int RoadmapId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DifficultyLevel Difficulty { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    /// <summary>Courses whose primary roadmap is this one (COURSE.primary_roadmap_id).</summary>
    public ICollection<Course> PrimaryCourses { get; set; } = new List<Course>();

    public ICollection<CourseRoadmap> CourseRoadmaps { get; set; } = new List<CourseRoadmap>();

    public ICollection<RoadmapObjective> Objectives { get; set; } = new List<RoadmapObjective>();

    public ICollection<RoadmapTopic> RoadmapTopics { get; set; } = new List<RoadmapTopic>();
}
