using OSS.Data;
using System.Threading.Tasks;

namespace OSS.Services.Events
{
    /// <summary>
    /// Represents an event publisher
    /// </summary>
    public partial interface IEventHandler<T> where T : EventBase
    {
         /// <summary>
         /// Publish event to consumers
         /// </summary>
         /// <typeparam name="TEvent">Type of event</typeparam>
         /// <param name="entity">Event object</param>
         Task RunAsync<TEvent>(TEvent entity);
     }
 }