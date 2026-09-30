namespace WebSpace.Models.Entities;

/// <summary>ERD: DISCUSSION_POST</summary>
public class DiscussionPost
{
    public int PostId { get; set; }

    public int CourseId { get; set; }

    /// <summary>Null means a General post for the whole course.</summary>
    public int? CourseWeekId { get; set; }

    public int AuthorUserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsPinned { get; set; }

    public bool IsClosed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Course Course { get; set; } = null!;

    public CourseWeek? CourseWeek { get; set; }

    public UserAccount Author { get; set; } = null!;

    public ICollection<DiscussionReply> Replies { get; set; } = new List<DiscussionReply>();
}
