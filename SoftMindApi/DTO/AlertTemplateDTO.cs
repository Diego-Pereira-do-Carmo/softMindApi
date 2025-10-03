namespace SoftMindApi.DTO
{
    public class CreateAlertTemplateDTO
    {
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class AlertTemplateDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
