using System;

namespace OSS.Services.Models
{
    public class LogModel : BaseModel
    {
        public int LogLevelId { get; set; }
        public string ShortMessage { get; set; } = string.Empty;
        public string FullMessage { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string PageUrl { get; set; } = string.Empty;
        public string ReferrerUrl { get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }
        public string CreatedOn { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
