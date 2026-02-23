using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.AppServices;
using OSS.Services.Models;

namespace OSS.Services.DomainServices
{
    public class LanguageService : ILanguageService
    {
        private readonly IRepository<Language> _repository;
        private readonly ICacheManager _cacheManager;
        public LanguageService(IRepository<Language> ctx, ICacheManager cacheManager)
        {
            _repository = ctx;
            _cacheManager = cacheManager;
        }
        public async Task<List<LanguageModel>> GetLanguagesAsync()
        {
            return await _cacheManager.Get(OSSDefaults.LanguagesAllCacheKey, async () =>
            {
                return await _repository.TableNoTracking
                    .OrderBy(a => a.DisplayOrder)
                    .Select(a => new LanguageModel
                    {
                        Id = a.Id,
                        Name = a.Name,
                        LanguageCulture = a.LanguageCulture,
                        DisplayOrder = a.DisplayOrder
                    })
                    .ToListAsync();
            });
        }
    }
}
