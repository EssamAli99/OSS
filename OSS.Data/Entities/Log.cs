using Microsoft.AspNetCore.Identity;
using System;

namespace OSS.Data.Entities
{
    public class Log : BaseEntity
    {
        public int LogLevelId { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public string IpAddress { get; set; }
        public string UserId { get; set; } //AspNetUsers
        public string PageUrl { get; set; }
        public string ReferrerUrl { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public IdentityUser User { get; set; }
    }
}
