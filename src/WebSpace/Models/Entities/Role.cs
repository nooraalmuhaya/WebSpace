namespace WebSpace.Models.Entities;

/// <summary>ERD: ROLE. Student, Tutor or Administrator (Guest is not stored).</summary>
public class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<UserAccount> Users { get; set; } = new List<UserAccount>();
}
