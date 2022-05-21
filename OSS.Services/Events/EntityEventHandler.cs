using OSS.Data.Entities;
using OSS.Services.DomainServices;
using System;
using System.Threading.Tasks;

namespace OSS.Services.Events
{
    /// <summary>
    /// Represents the event publisher implementation
    /// </summary>
    public partial class EntityEventHandler<T> : IEventHandler<T> where T : EventBase
    {
        private readonly ITestTableService _service;
        public EntityEventHandler(ITestTableService service)
        {
            _service = service;
        }

        #region Methods

        /// <summary>
        /// Publish event to consumers
        /// </summary>
        /// <typeparam name="TEvent">Type of event</typeparam>
        /// <param name="entity">Event object</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task RunAsync<TEvent>(TEvent entity)
        {
            var total = _service.GetTotal(null);
            var x = entity as EntityUpdatedEvent<OSS.Data.BaseEntity>;
            if (x.Entity is TestTable) Console.WriteLine(x.Entity.Id);
            return Task.Run(() =>
            {
                Console.WriteLine($"from entity eventhandler -> eventName is: {nameof(TEvent)}");
                Console.WriteLine($"from entity eventhandler -> entity is: {nameof(entity)}");
            });

        }

        #endregion
    }
}