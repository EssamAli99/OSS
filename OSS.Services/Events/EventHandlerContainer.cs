using Microsoft.Extensions.DependencyInjection;
using OSS.Data.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.Events
{
    public class EventHandlerContainer : IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventHandlerContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<T>(T @event) where T : EventBase
        {
            // Fix-11: Resolve handlers dynamically via DI instead of static dictionary
            var handlers = _serviceProvider.GetServices<IEventHandler<T>>();
            foreach (var handler in handlers)
            {
                await handler.RunAsync(@event);
            }
        }
    }
}
