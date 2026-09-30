using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: ASSIGNMENT. One assignment per course week.</summary>
public class Assignment
{
    public int AssignmentId { get; set; }

    public int CourseWeekId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public AssignmentType AssignmentType { get; set; }

    public DateTime? DueAt { get; set; }

    /// <summary>For example ".html,.css,.zip,.pdf".</summary>
    public string? AcceptedFileTypes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public CourseWeek CourseWeek { get; set; } = null!;

    public ICollection<AssignmentRequirement> Requirements { get; set; } = new List<AssignmentRequirement>();

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
