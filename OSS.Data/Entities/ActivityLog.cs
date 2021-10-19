using System;

namespace OSS.Data.Entities
{
    public class ActivityLog : BaseEntity
    {
        public int ActivityLogTypeId { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public virtual string IpAddress { get; set; }
    }
}
