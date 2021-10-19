using Microsoft.EntityFrameworkCore;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace OSS.Services.DomainServices
{
    public class AppPageService : IAppPageService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ICacheManager _cacheManager;
        public AppPageService(ApplicationDbContext ctx, ICacheManager cacheManager)
        {
            _ctx = ctx;
            _cacheManager = cacheManager;
        }
        public List<AppPageModel> GetAppPages(string allowedPaged)
        {
            var pages = _cacheManager.Get(OSSDefaults.ApplicationPagesCacheKey, () =>
            {
                return _ctx.AppPage.Include(x => x.AppPageActions).AsNoTracking().OrderBy(a => a.PageOrder).ToList();
            });

            return pages.Where(x => allowedPaged.Contains(x.SystemName))
                .Select(x => new AppPageModel
                {
                    Id = x.Id,
                    ActionName = x.ActionName,
                    AppPageId = x.AppPageId,
                    ControllerName = x.ControllerName,
                    IconClass = x.IconClass,
                    PageOrder = x.PageOrder,
                    PermissionNames = x.PermissionNames,
                    SystemName = x.SystemName,
                    Title = x.Title,
                    AreaName = x.AreaName
                }).ToList();
        }
        public List<AppPageModel> GetAppPages()
        {
            var pages = _cacheManager.Get(OSSDefaults.ApplicationPagesCacheKey, () =>
            {
                return _ctx.AppPage.Include(x=> x.AppPageActions).AsNoTracking().OrderBy(a => a.PageOrder).ToList();
            });

            return pages.Select(x => new AppPageModel
            {
                Id = x.Id,
                ActionName = x.ActionName,
                AppPageId = x.AppPageId,
                ControllerName = x.ControllerName,
                IconClass = x.IconClass,
                PageOrder = x.PageOrder,
                PermissionNames = x.PermissionNames,
                SystemName = x.SystemName,
                Title = x.Title,
                AreaName = x.AreaName
            }).ToList();
        }
    }
}
