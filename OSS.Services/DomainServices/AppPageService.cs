using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.DomainServices
{
    public class AppPageService : IAppPageService
    {
        private readonly IRepository<AppPage> _repository;
        private readonly ICacheManager _cacheManager;
        public AppPageService(IRepository<AppPage> ctx, ICacheManager cacheManager)
        {
            _repository = ctx;
            _cacheManager = cacheManager;
        }

        public async Task<List<AppPageModel>> GetAppPagesAsync(string allowedPaged)
        {
            var pages = await _cacheManager.Get(OSSDefaults.ApplicationPagesCacheKey, async () =>
            {
                return await _repository.TableNoTracking.Include(x => x.AppPageActions).OrderBy(a => a.PageOrder).ToListAsync();
            });

            pages = pages.Where(x => allowedPaged.Contains(x.SystemName)).ToList();
            
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

        public async Task<List<AppPageModel>> GetAppPagesAsync()
        {
            var pages = await _cacheManager.Get(OSSDefaults.ApplicationPagesCacheKey, async () =>
            {
                return await _repository.TableNoTracking.Include(x => x.AppPageActions).OrderBy(a => a.PageOrder).ToListAsync();
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
