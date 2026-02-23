namespace OSS.Data.Entities
{
    public class ActivityLogType : BaseEntity
    {
        public string SystemKeyword { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }
}
