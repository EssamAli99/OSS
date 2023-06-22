using Microsoft.AspNetCore.Mvc;
using OSS.Services;
using OSS.Services.DomainServices;
using OSS.Services.ExportImport;
using OSS.Services.Models;
using OSS.Web.Models;
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
            var param = GetParameters();
            var data = await _service.PrepareModePagedList(param, true);
            return Ok(new BasePagedListModel<TestTableModel>
            {
                draw = data.PageIndex.ToString(),
                recordsFiltered = data.PageSize,
                recordsTotal = data.Count,
                data = data.ToList()
            });

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
            var lst = await _service.PrepareModePagedList(null, true);
            if (lst == null) return NotFound();
            var bytes = await _exportservice.ExportTestTablesToXlsxAsync(lst.ToList());
            return File(bytes, MimeTypes.TextXlsx, "TestTables.xlsx");
        }
    }
}
