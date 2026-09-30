namespace WebSpace.Models.Entities;

/// <summary>ERD: ASSIGNMENT_REQUIREMENT</summary>
public class AssignmentRequirement
{
    public int RequirementId { get; set; }

    public int AssignmentId { get; set; }

    public string RequirementText { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public Assignment Assignment { get; set; } = null!;
}
