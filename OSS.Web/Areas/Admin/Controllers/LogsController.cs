using Microsoft.AspNetCore.Mvc;
using OSS.Services;
using OSS.Services.AppServices;
using OSS.Services.Models;
using OSS.Web.Models;
using System.Collections.Generic;
using System.Linq;

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
        public IActionResult GetList()
        {
            //if (!AllowedPermissions.Contains(Permissions.READ)) return StatusCode(403);
            var q = this.Request.Query; // if called with get
            var f = this.Request.Form; // if called with post
            var currentPage = 1;
            IEnumerable<KeyValuePair<string, string>> param = null;
            if (q.Any())
            {
                param = q.Select(x =>
                {
                    return new KeyValuePair<string, string>(x.Key, x.Value.ToString());
                });
            }
            else
            {
                if (f.Any())
                    param = f.Select(x =>
                    {
                        return new KeyValuePair<string, string>(x.Key, x.Value.ToString());
                    });
            }
            //var data = _service.PrepareModePagedList(param);
            var data = _Logger.GetAll();
            var count = data.Count();
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
        public IActionResult Delete(string id)
        {
            //if (!AllowedPermissions.Contains(Permissions.DELET)) return StatusCode(403);
            var log = _Logger.PrepareModel(id);
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
