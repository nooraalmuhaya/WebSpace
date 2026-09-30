namespace WebSpace.Models.Entities;

/// <summary>ERD: ACTIVITY_LOG</summary>
public class ActivityLog
{
    public int ActivityId { get; set; }

    /// <summary>Null means the action was performed by the system.</summary>
    public int? UserId { get; set; }

    /// <summary>For example "Created Submission".</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>For example "Course" or "Lesson".</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>For example "SUB-00482".</summary>
    public string RecordLabel { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public UserAccount? User { get; set; }
}
