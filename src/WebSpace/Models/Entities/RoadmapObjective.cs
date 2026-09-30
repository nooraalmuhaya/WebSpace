namespace WebSpace.Models.Entities;

/// <summary>ERD: ROADMAP_OBJECTIVE</summary>
public class RoadmapObjective
{
    public int ObjectiveId { get; set; }

    public int RoadmapId { get; set; }

    public string ObjectiveText { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public Roadmap Roadmap { get; set; } = null!;
}
