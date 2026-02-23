using OSS.Services.AppServices;

namespace OSS.Web.Framework
{
    /// <summary>
    /// Represents a task to clear [Log] table
    /// </summary>
    public class ClearLogTask : IScheduleTask
    {


        private readonly ILogService _logger;





        public ClearLogTask(ILogService logger)
        {
            _logger = logger;
        }





        /// <summary>
        /// Executes a task
        /// </summary>
        public virtual async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _logger.ClearLogAsync();
        }


    }
}
