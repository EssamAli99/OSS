namespace OSS.Data.Entities
{
    public class AppPageAction : BaseEntity
    {
        public int AppPageId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? HtmlElementId { get; set; }
        public virtual AppPage AppPage { get; set; } = null!;
    }
}

