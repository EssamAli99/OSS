using OSS.Data.Entities;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OSS.Web.Framework
{
    /// <summary>
    /// Represents task thread
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public partial class TaskThread : IDisposable
    {
        #region Fields

        private readonly string _scheduleTaskUrl;
        private readonly int? _timeout;
        private readonly Dictionary<string, string> _tasks;
        private Timer _timer;
        private bool _disposed;
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        internal TaskThread(IHttpClientFactory client, ILogger logger, int seconds)
        {
            //Seconds = 10 * 60; //for testing
            _tasks = new Dictionary<string, string>();
            _scheduleTaskUrl = OSSConfig.ScheduleTaskPath; //$"{EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync().Result.Url.TrimEnd('/')}/{NopTaskDefaults.ScheduleTaskPath}";
            _timeout = OSSConfig.ScheduleTaskRunTimeout;//EngineContext.Current.Resolve<AppSettings>().CommonConfig.ScheduleTaskRunTimeout;

            _httpClient = client;
            _logger = logger;
            //var workContext = _context.HttpContext.RequestServices.GetService(typeof(IWorkContext)) as IWorkContext;
            //if (workContext != null) langId = workContext.WorkingLanguageId;
            this.Seconds = seconds;
        }

        #endregion

        #region Utilities

        private async Task RunAsync()
        {
            if (Seconds <= 0)
                return;

            StartedUtc = DateTime.UtcNow;
            IsRunning = true;
            HttpClient client = _httpClient.CreateClient(OSSConfig.DefaultHttpClient); //null;

            foreach (var taskName in _tasks.Keys)
            {
                var taskType = _tasks[taskName];
                try
                {
                    //create and configure client
                    //client = EngineContext.Current.Resolve<IHttpClientFactory>().CreateClient(OSSConfig.DefaultHttpClient);
                    if (_timeout.HasValue)
                        client.Timeout = TimeSpan.FromMilliseconds(_timeout.Value);

                    //send post data
                    var data = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>(nameof(taskType), taskType) });
                    await client.PostAsync(_scheduleTaskUrl, data);
                }
                catch (Exception ex)
                {
                   // var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                   // using var scope = serviceScopeFactory.CreateScope();
                    // Resolve
                    //var logger = _context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger; //EngineContext.Current.Resolve<ILogger>(scope);
                    //var localizationService = _context.HttpContext.RequestServices.GetService(typeof(ILocalizationService)) as ILocalizationService; // EngineContext.Current.Resolve<ILocalizationService>(scope);
                    //var webHelper = _context.HttpContext.RequestServices.GetService(typeof(IWebHelper)) as IWebHelper; //webHelper;
                    var message = ex.InnerException?.GetType() == typeof(TaskCanceledException) ? "ScheduleTasks TimeoutError" : ex.Message;
                    message = string.Format("ScheduleTasks Error: Task Name: {0}, Message: {1}, Task Type: {2}, URL: {3}", taskName, message, taskType, _scheduleTaskUrl);
                    string userId = ""; // _context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

                    _logger.Insert(new LogModel
                    {
                        FullMessage = ex?.ToString()?? "",
                        LogLevelId = (int)LogLevel.Error,
                        ShortMessage = message,
                        UserId = userId,
                        //IpAddress = _webHelper.GetCurrentIpAddress(),
                        //PageUrl = _webHelper.GetThisPageUrl(true),
                        //ReferrerUrl = _webHelper.GetUrlReferrer()
                    });
                }
                finally
                {
                    if (client != null)
                    {
                        client.Dispose();
                        client = null;
                    }
                }
            }

            IsRunning = false;
        }

        private void TimerHandler(object state)
        {
            try
            {
                _timer.Change(-1, -1);

                RunAsync().Wait();
            }
            catch
            {
                // ignore
            }
            finally
            {
                if (RunOnlyOnce)
                    Dispose();
                else
                    _timer.Change(Interval, Interval);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes the instance
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                lock (this)
                    _timer?.Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Inits a timer
        /// </summary>
        public void InitTimer()
        {
            if (_timer == null)
                _timer = new Timer(TimerHandler, null, InitInterval, Interval);
        }

        /// <summary>
        /// Adds a task to the thread
        /// </summary>
        /// <param name="task">The task to be added</param>
        public void AddTask(ScheduleTask task)
        {
            if (!_tasks.ContainsKey(task.Name))
                _tasks.Add(task.Name, task.Type);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the interval in seconds at which to run the tasks
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Get or set the interval before timer first start 
        /// </summary>
        public int InitSeconds { get; set; }

        /// <summary>
        /// Get or sets a datetime when thread has been started
        /// </summary>
        public DateTime StartedUtc { get; private set; }

        /// <summary>
        /// Get or sets a value indicating whether thread is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the interval (in milliseconds) at which to run the task
        /// </summary>
        public int Interval
        {
            get
            {
                //if somebody entered more than "2147483" seconds, then an exception could be thrown (exceeds int.MaxValue)
                var interval = Seconds * 1000;
                if (interval <= 0)
                    interval = int.MaxValue;
                return interval;
            }
        }

        /// <summary>
        /// Gets the due time interval (in milliseconds) at which to begin start the task
        /// </summary>
        public int InitInterval
        {
            get
            {
                //if somebody entered less than "0" seconds, then an exception could be thrown
                var interval = InitSeconds * 1000;
                if (interval <= 0)
                    interval = 0;
                return interval;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the thread would be run only once (on application start)
        /// </summary>
        public bool RunOnlyOnce { get; set; }

        #endregion
    }
}
