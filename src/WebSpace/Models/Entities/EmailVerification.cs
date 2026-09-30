namespace WebSpace.Models.Entities;

/// <summary>ERD: EMAIL_VERIFICATION</summary>
public class EmailVerification
{
    public int VerificationId { get; set; }

    public int UserId { get; set; }

    /// <summary>Registration or changed email.</summary>
    public string TargetEmail { get; set; } = string.Empty;

    /// <summary>6-digit code, hashed.</summary>
    public string CodeHash { get; set; } = string.Empty;

    /// <summary>created_at + 15 minutes.</summary>
    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserAccount User { get; set; } = null!;
}
