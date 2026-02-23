using System.Threading.Tasks;

namespace OSS.Data.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : EventBase;
    }
}
