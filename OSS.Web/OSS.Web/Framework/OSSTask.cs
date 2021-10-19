using Microsoft.AspNetCore.Http;
using OSS.Data.Entities;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace OSS.Web.Framework
{
    /// <summary>
    /// Task
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public partial class OSSTask
    {
        #region Fields

        private int langId = 1;
        private bool? _enabled;
        private readonly HttpContext _context;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly ILocker _locker;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public OSSTask(ScheduleTask task, HttpContext context)//, IScheduleTaskService scheduleTaskService, ILogger logger, ILocalizationService localization, ILocker locker, IWebHelper webHelper)
        {
            ScheduleTask = task;
            _context = context;
            _scheduleTaskService = _context.RequestServices.GetService(typeof(IScheduleTaskService)) as IScheduleTaskService; //scheduleTaskService;
            _logger = _context.RequestServices.GetService(typeof(ILogger)) as ILogger; //logger;
            _localizationService = _context.RequestServices.GetService(typeof(ILocalizationService)) as ILocalizationService; //localization;
            _locker = _context.RequestServices.GetService(typeof(ILocker)) as ILocker; //locker;
            _webHelper = _context.RequestServices.GetService(typeof(IWebHelper)) as IWebHelper; //webHelper;
            var workContext = _context.RequestServices.GetService(typeof(IWorkContext)) as IWorkContext;
            if (workContext != null) langId = workContext.WorkingLanguageId;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Initialize and execute task
        /// </summary>
        private void ExecuteTask()
        {

            if (!Enabled)
                return;

            var type = Type.GetType(ScheduleTask.Type) ??
                //ensure that it works fine when only the type name is specified (do not require fully qualified names)
                AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(ScheduleTask.Type))
                .FirstOrDefault(t => t != null);
            if (type == null)
                throw new Exception($"Schedule task ({ScheduleTask.Type}) cannot by instantiated");

            object instance = null;
            try
            {
                instance = _context.RequestServices.GetService(type);
            }
            catch
            {
                //try resolve
            }

            if (instance == null)
                //not resolved
                instance = _webHelper.ResolveUnregistered(type);

            if (instance is not IScheduleTask task)
                return;

            ScheduleTask.LastStartUtc = DateTime.UtcNow;
            //update appropriate datetime properties
            _scheduleTaskService.UpdateAsync(ScheduleTask).Wait();
            task.ExecuteAsync().Wait();
            ScheduleTask.LastEndUtc = ScheduleTask.LastSuccessUtc = DateTime.UtcNow;
            //update appropriate datetime properties
            _scheduleTaskService.UpdateAsync(ScheduleTask).Wait();
        }

        /// <summary>
        /// Is task already running?
        /// </summary>
        /// <param name="scheduleTask">Schedule task</param>
        /// <returns>Result</returns>
        protected virtual bool IsTaskAlreadyRunning(ScheduleTask scheduleTask)
        {
            //task run for the first time
            if (!scheduleTask.LastStartUtc.HasValue && !scheduleTask.LastEndUtc.HasValue)
                return false;

            var lastStartUtc = scheduleTask.LastStartUtc ?? DateTime.UtcNow;

            //task already finished
            if (scheduleTask.LastEndUtc.HasValue && lastStartUtc < scheduleTask.LastEndUtc)
                return false;

            //task wasn't finished last time
            if (lastStartUtc.AddSeconds(scheduleTask.Seconds) <= DateTime.UtcNow)
                return false;

            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="ensureRunOncePerPeriod">A value indicating whether we should ensure this task is run once per run period</param>
        public async System.Threading.Tasks.Task ExecuteAsync(bool throwException = false, bool ensureRunOncePerPeriod = true)
        {
            if (ScheduleTask == null || !Enabled)
                return;

            if (ensureRunOncePerPeriod)
            {
                //task already running
                if (IsTaskAlreadyRunning(ScheduleTask))
                    return;

                //validation (so nobody else can invoke this method when he wants)
                if (ScheduleTask.LastStartUtc.HasValue && (DateTime.UtcNow - ScheduleTask.LastStartUtc).Value.TotalSeconds < ScheduleTask.Seconds)
                    //too early
                    return;
            }

            try
            {
                //get expiration time
                var expirationInSeconds = Math.Min(ScheduleTask.Seconds, 300) - 1;
                var expiration = TimeSpan.FromSeconds(expirationInSeconds);

                //execute task with lock
                _locker.PerformActionWithLock(ScheduleTask.Type, expiration, ExecuteTask);
            }
            catch (Exception exc)
            {
                var scheduleTaskUrl = OSSConfig.ScheduleTaskPath;

                ScheduleTask.Enabled = !ScheduleTask.StopOnError;
                ScheduleTask.LastEndUtc = DateTime.UtcNow;
                await _scheduleTaskService.UpdateAsync(ScheduleTask);

                var message = string.Format(_localizationService.GetResource("ScheduleTasks.Error", langId), ScheduleTask.Name,
                    exc.Message, ScheduleTask.Type, scheduleTaskUrl);

                //log error
                string userId = _context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var m = new LogModel
                {
                    LogLevelId = (int)LogLevel.Error,
                    ShortMessage = message,
                    UserId = userId,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    PageUrl = _webHelper.GetThisPageUrl(true),
                    ReferrerUrl = _webHelper.GetUrlReferrer()
                };
                _logger.Insert(m);

                if (throwException)
                    throw;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Schedule task
        /// </summary>
        public ScheduleTask ScheduleTask { get; }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                if (!_enabled.HasValue)
                    _enabled = ScheduleTask?.Enabled;

                return _enabled.HasValue && _enabled.Value;
            }

            set => _enabled = value;
        }

        #endregion
    }
}
