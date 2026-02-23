using System;

namespace OSS.Data.Events
{
    public class EventBase
    {
        public EventBase()
        {
            OccuredOn = DateTime.Now;
        }
        public string Message { get; set; } = string.Empty;
        protected DateTime OccuredOn { get; set; }
    }
}
