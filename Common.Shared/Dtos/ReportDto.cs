namespace Common.Shared.Dtos
{
    public class ReportDto
    {
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string? AttachmentPath { get; set; }
    }
}
