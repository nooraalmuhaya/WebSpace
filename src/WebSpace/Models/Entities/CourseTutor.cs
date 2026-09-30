namespace WebSpace.Models.Entities;

/// <summary>ERD: COURSE_TUTOR. Composite key (course_id, tutor_user_id).</summary>
public class CourseTutor
{
    public int CourseId { get; set; }

    public int TutorUserId { get; set; }

    public DateTime AssignedAt { get; set; }

    public Course Course { get; set; } = null!;

    public UserAccount Tutor { get; set; } = null!;
}
