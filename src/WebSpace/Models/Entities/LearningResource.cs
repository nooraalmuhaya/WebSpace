using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: LEARNING_RESOURCE</summary>
public class LearningResource
{
    public int ResourceId { get; set; }

    public int LessonId { get; set; }

    public string Title { get; set; } = string.Empty;

    public ResourceType ResourceType { get; set; }

    /// <summary>Used for Video and Link resources.</summary>
    public string? ResourceUrl { get; set; }

    /// <summary>Used for File resources.</summary>
    public string? FilePath { get; set; }

    public long? FileSizeBytes { get; set; }

    public string? Description { get; set; }

    public Lesson Lesson { get; set; } = null!;
}
