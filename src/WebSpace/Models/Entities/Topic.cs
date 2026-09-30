namespace WebSpace.Models.Entities;

/// <summary>ERD: TOPIC. For example HTML, CSS or SQL.</summary>
public class Topic
{
    public int TopicId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<RoadmapTopic> RoadmapTopics { get; set; } = new List<RoadmapTopic>();
}
