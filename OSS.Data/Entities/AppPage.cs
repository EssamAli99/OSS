namespace OSS.Data.Entities
{
    public class AppPage : BaseEntity
    {
        public AppPage()
        {
            ChildNodes = new HashSet<AppPage>();
            AppPageActions = new HashSet<AppPageAction>();
        }
        public string SystemName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string IconClass { get; set; } = string.Empty;
        public string PermissionNames { get; set; } = string.Empty;
        public int PageOrder { get; set; }
        public int? AppPageId { get; set; }
        public string? AreaName { get; set; }
        public ICollection<AppPage> ChildNodes { get; set; }
        public ICollection<AppPageAction> AppPageActions { get; set; }
    }
}

