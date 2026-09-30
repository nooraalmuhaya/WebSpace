namespace WebSpace.Models.Entities;

/// <summary>ERD: ROADMAP_TOPIC. Composite key (roadmap_id, topic_id).</summary>
public class RoadmapTopic
{
    public int RoadmapId { get; set; }

    public int TopicId { get; set; }

    public Roadmap Roadmap { get; set; } = null!;

    public Topic Topic { get; set; } = null!;
}
