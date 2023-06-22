using Microsoft.AspNetCore.Mvc;
using OSS.Services;
using OSS.Services.AppServices;
using OSS.Services.Models;
using OSS.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Areas.Admin.Controllers
{
    public class LogsController : AdminBaseController
    {
        private readonly ILogger _Logger;
        public LogsController(ILogger logService)
        {
            _Logger = logService;
        }
        public IActionResult Index()
        {
            //if (!IsPageAllowed) return StatusCode(403);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetList()
        {
            var param = GetParameters();
            //var data = _service.PrepareModePagedList(param);
            var data = await _Logger.GetAll();
            var count = data.Count();
            var currentPage = 1;
            if (param != null && param.Any(x => x.Key == "draw"))
            {
                currentPage = int.Parse(param.FirstOrDefault(x => x.Key == "draw").Value);
            }
            var jsonData = new BasePagedListModel<LogModel>
            {
                draw = currentPage.ToString(),
                recordsFiltered = count,
                recordsTotal = count,
                data = data
            };
            return Ok(jsonData);

        }
        public async Task<IActionResult> Delete(string id)
        {
            //if (!AllowedPermissions.Contains(Permissions.DELET)) return StatusCode(403);
            var log = await _Logger.PrepareModel(id);
            if (log == null) return NotFound();
            log.ModelMode = ModelActions.Delete;
            return View("Edit", log);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            //if (!AllowedPermissions.Contains(Permissions.DELET)) return StatusCode(403);
            _Logger.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
