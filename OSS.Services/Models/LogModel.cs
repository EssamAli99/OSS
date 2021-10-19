using System;

namespace OSS.Services.Models
{
    public class LogModel : BaseModel
    {
        public int LogLevelId { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public string IpAddress { get; set; }
        public string UserId { get; set; }
        public string PageUrl { get; set; }
        public string ReferrerUrl { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string CreatedOn { get; set; }
        public string Email { get; set; }
    }
}
