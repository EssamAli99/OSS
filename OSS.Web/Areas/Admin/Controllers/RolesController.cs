using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OSS.Services;
using OSS.Services.DomainServices;
using OSS.Web.Areas.Admin.Models;
using OSS.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OSS.Web.Areas.Admin.Controllers
{
    public class RolesController : AdminBaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAppPageService _appPageService;

        public RolesController(RoleManager<IdentityRole> rm, IAppPageService aps) 
        {
            _roleManager = rm;
            _appPageService = aps;
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
            var f = this.Request.Form;
            var currentPage = 1;
            IEnumerable<KeyValuePair<string, string>> param = null;

            if (f.Any())
                param = f.Select(x =>
                {
                    return new KeyValuePair<string, string>(x.Key, x.Value.ToString());
                });

            var lst = _roleManager.Roles.ToList();
            if (param != null && param.Any(x => x.Key == "draw"))
            {
                currentPage = int.Parse(param.FirstOrDefault(x => x.Key == "draw").Value);
            }
            var jsonData = new BasePagedListModel<RoleModel>
            {
                draw = currentPage.ToString(),
                recordsFiltered = lst.Count,
                recordsTotal = lst.Count,
                data = lst.Select(x => new RoleModel
                {
                    EncrypedId = x.Id,
                    Id = x.Id,
                    ModelMode = ModelActions.List,
                    Name = x.Name
                })
            };
            return Ok(jsonData);
        }
        
        public async Task<IActionResult> Create()
        {
            var pages = await _appPageService.GetAppPagesAsync();
            pages = pages.Where(x => !string.IsNullOrEmpty(x.ControllerName)).ToList();
            var model = new RoleModel
            {
                ModelMode = ModelActions.Add,
                AppPages = pages.Select(x => new AppPageModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    ControllerName = x.ControllerName,
                    Permissions = x.PermissionNames.Split(",").Select(y => new SelectListItem
                    {
                        Text = OSSConfig.PermissionNames[y],
                        Value = y
                    }).ToList()
                }).ToList()
            };

            return View("Edit", model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleModel model)
        {
            //if (!AllowedPermissions.Contains(Permissions.INSERT)) return StatusCode(403);
            if (!string.IsNullOrEmpty(model.Name))
            {
                var role = new IdentityRole(model.Name);
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    var pagesWithPermissions = model.AppPages.Where(x => x.Permissions.Any(y => y.Selected)).ToList();
                    if (pagesWithPermissions != null && pagesWithPermissions.Count > 0)
                    {
                        foreach (var item in pagesWithPermissions)
                        {
                            await _roleManager.AddClaimAsync(role,
                                new Claim(item.ControllerName, string.Join(",", item.Permissions.Where(x => x.Selected).Select(x => x.Value))));
                        }
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(ModelActions m, string id)
        {
            //if (!AllowedPermissions.Contains(Permissions.UPDATE)) return StatusCode(403);
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == id);
            if (role == null) return NotFound();
            var claims = await _roleManager.GetClaimsAsync(role);
            var pages = await _appPageService.GetAppPagesAsync();
            pages = pages.Where(x=> !string.IsNullOrEmpty(x.ControllerName)).ToList();
            var model = new RoleModel
            {
                EncrypedId = role.Id,
                Id = role.Id,
                ModelMode = m,
                Name = role.Name,
                //AppPages = new List<AppPageModel>()
                AppPages = pages.Select(x => new AppPageModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    ControllerName = x.ControllerName,
                    Permissions = x.PermissionNames.Split(",").Select(y => new SelectListItem
                    {
                        Text = OSSConfig.PermissionNames[y],
                        Value = y,
                        Disabled = (m == ModelActions.Delete || m == ModelActions.List) ,
                        Selected = (claims.FirstOrDefault(c => c.Type.Equals(x.ControllerName) && c.Value.Contains(y)) != null)
                    }).ToList()
                }).ToList()
            };

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoleModel model)
        {
            //if (!AllowedPermissions.Contains(Permissions.UPDATE)) return StatusCode(403);
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == model.EncrypedId);
            if (role != null)
            {
                if (role.Name != model.Name)
                {
                    role.Name = model.Name;
                    await _roleManager.UpdateAsync(role);
                }
                // use role manager
                var oldClaims = await _roleManager.GetClaimsAsync(role);
                var pagesWithPermissions = model.AppPages.Where(x => x.Permissions.Any(y => y.Selected)).ToList();
                if (pagesWithPermissions == null || pagesWithPermissions.Count == 0)
                {
                    if (oldClaims != null && oldClaims.Count > 0)
                    {
                        foreach (var item in oldClaims)
                        {
                            await _roleManager.RemoveClaimAsync(role, item);
                        }
                    }
                }
                else
                {
                    foreach (var item in pagesWithPermissions)
                    {
                        var v = string.Join(",", item.Permissions.Where(x => x.Selected).Select(x => x.Value));
                        var c = oldClaims.FirstOrDefault(x => x.Type == item.ControllerName);
                        if (c != null)
                        {
                            await _roleManager.RemoveClaimAsync(role, c);
                        }
                        await _roleManager.AddClaimAsync(role,
                            new Claim(item.ControllerName, v));
                    }
                }

                //var oldClaims = _identityService.GetRoleClaims(role.Id);
                //var pagesWithPermissions = model.AppPages.Where(x => x.Permissions.Any(y => y.Selected)).ToList();
                //if (pagesWithPermissions == null || pagesWithPermissions.Count == 0)
                //{
                //    if (oldClaims != null && oldClaims.Count > 0)
                //    {
                //        _identityService.Delete(oldClaims);
                //    }
                //}
                //else
                //{
                //    foreach (var item in pagesWithPermissions)
                //    {
                //        var v = string.Join(",", item.Permissions.Where(x => x.Selected).Select(x => x.Value));
                //        var c = oldClaims.FirstOrDefault(x => x.ClaimType == item.ControllerName);
                //        if (c != null)
                //        {
                //            c.ClaimValue = v;
                //        }
                //        else
                //        {
                //            c = new Data.Entities.AspNetRoleClaims
                //            {
                //                ClaimType = item.ControllerName,
                //                ClaimValue = v,
                //                RoleId = role.Id
                //            };
                //        }
                //        c.Id = _identityService.Save(c);
                //    }
                //}

            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            //if (!AllowedPermissions.Contains(PermissionIds.DELET)) return StatusCode(403);
            var role = _roleManager.Roles.FirstOrDefault(x => x.Id == id);
            if (role != null) await _roleManager.DeleteAsync(role);
            return RedirectToAction(nameof(Index));
        }
    }
}
