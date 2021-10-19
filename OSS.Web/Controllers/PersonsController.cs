using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSS.Services;
using OSS.Services.DomainServices;
using OSS.Services.Models;
using OSS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Controllers
{
    [Authorize]
    public class PersonsController :Controller
    {
        private readonly IPersonService _service;

        public PersonsController(IPersonService service)
        {
            _service = service;
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
            var jsonData = new BasePagedListModel<PersonModel>
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
            var person = new PersonModel
            {
                ModelMode = ModelActions.Add
            };
            return View("Edit", person);
        }

        public IActionResult Edit(string id)
        {
            var person = _service.PrepareMode(id);
            if (person == null) return NotFound();
            person.ModelMode = ModelActions.Edit;
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PersonModel personModel)
        {
            if (id != personModel.EncrypedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _service.Save(personModel);
                return RedirectToAction(nameof(Index));
            }
            return View(personModel);
        }

        // GET: TestTables/Delete/5
        public IActionResult Delete(string id)
        {
            var person = _service.PrepareMode(id);
            if (person == null) return NotFound();
            person.ModelMode = ModelActions.Delete;
            return View("Edit", person);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            _service.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
