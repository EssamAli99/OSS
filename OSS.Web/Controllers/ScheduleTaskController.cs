using Microsoft.AspNetCore.Mvc;
using OSS.Services.AppServices;
using OSS.Web.Framework;
using System.Threading.Tasks;

namespace OSS.Web.Controllers
{
    public class ScheduleTaskController : Controller
    {
        private readonly IScheduleTaskService _scheduleTaskService;

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> RunTask(string taskType)
        {
            var scheduleTask = await _scheduleTaskService.GetByTypeAsync(taskType);
            if (scheduleTask == null)
                //schedule task cannot be loaded
                return NoContent();

            var task = new OSSTask(scheduleTask, this.HttpContext);
            await task.ExecuteAsync();

            return NoContent();
        }
    }
}
