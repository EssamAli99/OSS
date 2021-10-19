namespace OSS.Services.Models
{
    public class AppPageModel
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IconClass { get; set; }
        public string PermissionNames { get; set; }
        public int PageOrder { get; set; }
        public int? AppPageId { get; set; }
        public string AreaName { get; set; }
    }
}
