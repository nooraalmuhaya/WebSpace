namespace WebSpace.Models.Entities;

/// <summary>ERD: SUBMISSION_FILE</summary>
public class SubmissionFile
{
    public int SubmissionFileId { get; set; }

    public int SubmissionId { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFilePath { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public DateTime UploadedAt { get; set; }

    public Submission Submission { get; set; } = null!;
}
