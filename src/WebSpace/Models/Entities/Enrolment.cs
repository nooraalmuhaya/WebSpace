using WebSpace.Models.Enums;

namespace WebSpace.Models.Entities;

/// <summary>ERD: ENROLMENT</summary>
public class Enrolment
{
    public int EnrolmentId { get; set; }

    public int StudentUserId { get; set; }

    public int CourseId { get; set; }

    public EnrolmentStatus Status { get; set; }

    public DateTime EnrolledAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? LastAccessedAt { get; set; }

    public UserAccount Student { get; set; } = null!;

    public Course Course { get; set; } = null!;
}
