using Microsoft.AspNetCore.Mvc;
using OSS.Services;
using OSS.Services.DomainServices;
using OSS.Services.ExportImport;
using OSS.Services.Models;
using OSS.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Controllers
{
    public class TestTablesController : BaseController
    {
        private readonly ITestTableService _service;
        private readonly IExportManager _exportservice;

        public TestTablesController(ITestTableService service, IExportManager exportservice)
        {
            _service = service;
            _exportservice = exportservice;
        }

        // GET: TestTables
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetList()
        {
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
            var data = await _service.PrepareModeListAsync(null);
            var count = await _service.GetTotal(null);
            if (param != null && param.Any(x => x.Key == "draw"))
            {
                currentPage = int.Parse(param.FirstOrDefault(x => x.Key == "draw").Value);
            }
            var jsonData = new BasePagedListModel<TestTableModel>
            {
                draw = currentPage.ToString(),
                recordsFiltered = count,
                recordsTotal = count,
                data = data
            };
            return Ok(jsonData);

        }

        // GET: TestTables/Create
        public IActionResult Create()
        {
            var testTable = new TestTableModel
            {
                ModelMode = ModelActions.Add
            };
            return View("Edit", testTable);
        }

        public async Task<IActionResult> Edit(string id)
        {
            //if (!AllowedPermissions.Contains(PermissionIds.UPDATE)) return StatusCode(401);
            var testTable = await _service.PrepareMode(id);
            if (testTable == null) return NotFound();
            testTable.ModelMode = ModelActions.Edit;
            return View(testTable);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TestTableModel testTable)
        {
            if (id != testTable.EncrypedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _service.Save(testTable);
                return RedirectToAction(nameof(Index));
            }
            return View(testTable);
        }

        // GET: TestTables/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var testTable = await _service.PrepareMode(id);
            if (testTable == null) return NotFound();
            testTable.ModelMode = ModelActions.Delete;
            return View("Edit", testTable);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            await _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Import()
        {
            return null;
        }

        //[HttpPost]
        public async Task<IActionResult> Export()
        {
            var lst = await _service.PrepareModeListAsync(null);
            if (lst == null) return NotFound();
            var bytes = await _exportservice.ExportTestTablesToXlsxAsync(lst.ToList());
            return File(bytes, MimeTypes.TextXlsx, "TestTables.xlsx");
        }
    }
}
