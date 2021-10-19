namespace OSS.Data.Entities
{
    public class ActivityLogType : BaseEntity
    {
        public string SystemKeyword { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
}
