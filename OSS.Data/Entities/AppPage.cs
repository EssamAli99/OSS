using System.Collections.Generic;

namespace OSS.Data.Entities
{
    public class AppPage : BaseEntity
    {
        public AppPage()
        {
            ChildNodes = new HashSet<AppPage>();
            AppPageActions = new HashSet<AppPageAction>();
        }
        public string SystemName { get; set; }
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string IconClass { get; set; }
        public string PermissionNames { get; set; }
        public int PageOrder { get; set; }
        public int? AppPageId { get; set; }
        public string AreaName { get; set; }
        public ICollection<AppPage> ChildNodes { get; set; }
        public ICollection<AppPageAction> AppPageActions { get; set; }
    }
}
