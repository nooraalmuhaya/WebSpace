namespace WebSpace.Models.Entities;

/// <summary>ERD: DISCUSSION_REPLY</summary>
public class DiscussionReply
{
    public int ReplyId { get; set; }

    public int PostId { get; set; }

    public int AuthorUserId { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DiscussionPost Post { get; set; } = null!;

    public UserAccount Author { get; set; } = null!;
}
