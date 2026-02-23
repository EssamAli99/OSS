using OSS.Data;
using OSS.Data.Events;
using System.Threading.Tasks;

namespace OSS.Services.Events
{
    /// <summary>
    /// Represents an event publisher
    /// </summary>
    public interface IEventHandler<T> where T : EventBase
    {
        /// <summary>
        /// Publish event to consumers
        /// </summary>
        /// <typeparam name="TEvent">Type of event</typeparam>
        /// <param name="entity">Event object</param>
        Task RunAsync<TEvent>(TEvent entity);
    }
}
