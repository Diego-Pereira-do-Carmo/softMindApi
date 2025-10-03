namespace SoftMindApi.DTO
{
    public class AlertDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class MarkAsReadDTO
    {
        public string AlertId { get; set; } = string.Empty;
    }

    public class CreateAlertDTO
    {
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}