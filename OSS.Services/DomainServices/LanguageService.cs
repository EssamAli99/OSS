using Microsoft.EntityFrameworkCore;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace OSS.Services.DomainServices
{
    public class LanguageService : ILanguageService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ICacheManager _cacheManager;
        public LanguageService(ApplicationDbContext ctx, ICacheManager cacheManager)
        {
            _ctx = ctx;
            _cacheManager = cacheManager;
        }
        public List<LanguageModel> GetLanguages()
        {
            return _cacheManager.Get(OSSDefaults.LanguagesAllCacheKey, () =>
            {
                return _ctx.Language.AsNoTracking()
                    .OrderBy(a => a.DisplayOrder)
                    .Select(a => new LanguageModel
                    {
                        Id = a.Id,
                        Name = a.Name
                    })
                    .ToList();
            });
        }
    }
}
