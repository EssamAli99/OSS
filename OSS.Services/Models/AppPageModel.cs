namespace OSS.Services.Models
{
    public class AppPageModel
    {
        public int Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string PermissionNames { get; set; } = string.Empty;
        public int PageOrder { get; set; }
        public int? AppPageId { get; set; }
        public string AreaName { get; set; } = string.Empty;
    }
}
