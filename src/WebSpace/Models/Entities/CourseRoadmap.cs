namespace WebSpace.Models.Entities;

/// <summary>ERD: COURSE_ROADMAP. Composite key (roadmap_id, course_id).</summary>
public class CourseRoadmap
{
    public int RoadmapId { get; set; }

    public int CourseId { get; set; }

    /// <summary>Unique per roadmap.</summary>
    public int SequencePosition { get; set; }

    public Roadmap Roadmap { get; set; } = null!;

    public Course Course { get; set; } = null!;
}
