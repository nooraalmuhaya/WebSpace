namespace WebSpace.Models.Entities;

/// <summary>ERD: PASSWORD_RESET_TOKEN</summary>
public class PasswordResetToken
{
    public int ResetTokenId { get; set; }

    public int UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    /// <summary>created_at + 30 minutes.</summary>
    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserAccount User { get; set; } = null!;
}
