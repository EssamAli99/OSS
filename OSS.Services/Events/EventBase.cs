using System;

namespace OSS.Services.Events
{
    public class EventBase
    {
        public EventBase()
        {
            OccuredOn = DateTime.Now;
        }
        public string Message { get; set; }
        protected DateTime OccuredOn { get; set; }
    }
}
