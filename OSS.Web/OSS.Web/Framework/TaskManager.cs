using OSS.Services.AppServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;

namespace OSS.Web.Framework
{
    /// <summary>
    /// Represents task manager
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public partial class TaskManager
    {
        #region Fields
        private readonly IScheduleTaskService _taskService;
        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        /// <returns>A task that represents the asynchronous operation</returns>
        public TaskManager(IScheduleTaskService scheduleTaskService, IHttpClientFactory client, ILogger logger)
        {
            _taskService = scheduleTaskService;  //_context.HttpContext.RequestServices.GetService(typeof(IScheduleTaskService)) as IScheduleTaskService; //scheduleTaskService;
            _httpClient = client;
            _logger = logger;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initializes the task manager
        /// </summary>
        public void Initialize()
        {
            _taskThreads.Clear();
            var scheduleTasks = _taskService
                .GetAllAsync().Result
                .OrderBy(x => x.Seconds)
                .ToList();

            foreach (var scheduleTask in scheduleTasks)
            {
                var taskThread = new TaskThread(_httpClient, _logger, scheduleTask.Seconds);

                //sometimes a task period could be set to several hours (or even days)
                //in this case a probability that it'll be run is quite small (an application could be restarted)
                //calculate time before start an interrupted task
                if (scheduleTask.LastStartUtc.HasValue)
                {
                    //seconds left since the last start
                    var secondsLeft = (DateTime.UtcNow - scheduleTask.LastStartUtc).Value.TotalSeconds;

                    if (secondsLeft >= scheduleTask.Seconds)
                        //run now (immediately)
                        taskThread.InitSeconds = 0;
                    else
                        //calculate start time
                        //and round it (so "ensureRunOncePerPeriod" parameter was fine)
                        taskThread.InitSeconds = (int)(scheduleTask.Seconds - secondsLeft) + 1;
                }
                else
                {
                    //first start of a task
                    taskThread.InitSeconds = scheduleTask.Seconds;
                }

                taskThread.AddTask(scheduleTask);
                _taskThreads.Add(taskThread);
            }
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in _taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in _taskThreads)
            {
                taskThread.Dispose();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the task manger instance
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        //public static TaskManager Instance { get; } = new TaskManager();

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public IList<TaskThread> TaskThreads => new ReadOnlyCollection<TaskThread>(_taskThreads);

        #endregion
    }
}
