using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Fields

        private readonly IRepository<LocaleStringResource> _repository;
        private readonly ICacheManager _cacheManager;
        public int WorkingLanguageId { get; set; }
        #endregion

        #region Ctor

        public LocalizationService(IRepository<LocaleStringResource> ctx, ICacheManager cacheManager)
        {
            this._repository = ctx;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Insert resources
        /// </summary>
        /// <param name="resources">Resources</param>
        protected virtual async Task InsertLocaleStringResources(IList<LocaleStringResource> resources)
        {
            if (resources == null)
                throw new ArgumentNullException(nameof(resources));

            //insert
            await _repository.InsertAsync(resources);
            //cache
            _cacheManager.RemoveByPattern(OSSDefaults.LocaleStringResourcesPatternCacheKey);

        }

        /// <summary>
        /// Update resources
        /// </summary>
        /// <param name="resources">Resources</param>
        protected virtual async Task UpdateLocaleStringResources(IList<LocaleStringResource> resources)
        {
            if (resources == null)
                throw new ArgumentNullException(nameof(resources));

            //update
            await _repository.UpdateAsync(resources);
            //cache
            _cacheManager.RemoveByPattern(OSSDefaults.LocaleStringResourcesPatternCacheKey);

        }

        private static Dictionary<string, KeyValuePair<int, string>> ResourceValuesToDictionary(IEnumerable<LocaleStringResource> locales)
        {
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var locale in locales)
            {
                var resourceName = locale.ResourceName.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceValue));
            }

            return dictionary;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual async Task DeleteLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException(nameof(localeStringResource));

            await _repository.DeleteAsync(localeStringResource);
            //cache
            _cacheManager.RemoveByPattern(OSSDefaults.LocaleStringResourcesPatternCacheKey);

        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceById(int localeStringResourceId)
        {
            if (localeStringResourceId == 0)
                return null;

            return await _repository.GetByIdAsync(localeStringResourceId);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <returns>Locale string resource</returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByName(string resourceName)
        {
                return await GetLocaleStringResourceByName(resourceName, WorkingLanguageId);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true)
        {
            var query = from lsr in _repository.TableNoTracking
                        orderby lsr.ResourceName
                        where lsr.LanguageId == languageId && lsr.ResourceName == resourceName
                        select lsr;
            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resources</returns>
        public virtual async Task<IList<LocaleStringResource>> GetAllResources(int languageId)
        {
            var query = from l in _repository.TableNoTracking
                        orderby l.ResourceName
                        where l.LanguageId == languageId
                        select l;
            return await query.ToListAsync();
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual async Task InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException(nameof(localeStringResource));

            await _repository.InsertAsync(localeStringResource);
            //cache
            _cacheManager.RemoveByPattern(OSSDefaults.LocaleStringResourcesPatternCacheKey);

        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual async Task UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            if (localeStringResource == null)
                throw new ArgumentNullException(nameof(localeStringResource));

            await _repository.UpdateAsync(localeStringResource);
            //cache
            _cacheManager.RemoveByPattern(OSSDefaults.LocaleStringResourcesPatternCacheKey);

        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="loadPublicLocales">A value indicating whether to load data for the public store only (if "false", then for admin area only. If null, then load all locales. We use it for performance optimization of the site startup</param>
        /// <returns>Locale string resources</returns>
        public virtual Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId, bool? loadPublicLocales)
        {
            var key = string.Format(OSSDefaults.LocaleStringResourcesAllCacheKey, languageId);

            //get all locale string resources by language identifier
            if (!loadPublicLocales.HasValue || _cacheManager.IsSet(key))
            {
                var rez = _cacheManager.Get(key, () =>
                {
                    //we use no tracking here for performance optimization
                    //anyway records are loaded only for read-only operations
                    var query = from l in _repository.TableNoTracking
                                orderby l.ResourceName
                                where l.LanguageId == languageId
                                select l;

                    return ResourceValuesToDictionary(query);
                });

                //remove separated resource 
                _cacheManager.Remove(string.Format(OSSDefaults.LocaleStringResourcesAllPublicCacheKey, languageId));
                _cacheManager.Remove(string.Format(OSSDefaults.LocaleStringResourcesAllAdminCacheKey, languageId));

                return rez;
            }

            //performance optimization of the site startup
            key = string.Format(loadPublicLocales.Value ? OSSDefaults.LocaleStringResourcesAllPublicCacheKey : OSSDefaults.LocaleStringResourcesAllAdminCacheKey, languageId);

            return _cacheManager.Get(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from l in _repository.TableNoTracking
                            orderby l.ResourceName
                            where l.LanguageId == languageId
                            select l;
                query = loadPublicLocales.Value ? query.Where(r => !r.ResourceName.StartsWith(OSSDefaults.AdminLocaleStringResourcesPrefix)) : query.Where(r => r.ResourceName.StartsWith(OSSDefaults.AdminLocaleStringResourcesPrefix));
                return ResourceValuesToDictionary(query);
            });
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>A string representing the requested resource string.</returns>
        //public virtual string GetResource(string resourceKey)
        //{
        //        return GetResource(resourceKey, WorkingLanguageId);
        //}

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether an empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        public virtual string GetResource(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();
            if (OSSDefaults.LoadAllLocaleRecordsOnStartup)
            {
                //load all records (we know they are cached)
                var resources = GetAllResourceValues(languageId, !resourceKey.StartsWith(OSSDefaults.AdminLocaleStringResourcesPrefix, StringComparison.InvariantCultureIgnoreCase));
                if (resources.ContainsKey(resourceKey))
                {
                    result = resources[resourceKey].Value;
                }
            }
            else
            {
                //gradual loading
                var key = string.Format(OSSDefaults.LocaleStringResourcesByResourceNameCacheKey, languageId, resourceKey);
                var lsr = _cacheManager.Get(key, () =>
                {
                    var query = from l in _repository.TableNoTracking
                                where l.ResourceName == resourceKey
                                && l.LanguageId == languageId
                                select l.ResourceValue;
                    return query.FirstOrDefault();
                });

                if (lsr != null)
                    result = lsr;
            }

            if (!string.IsNullOrEmpty(result)) 
                return result;

            if (!string.IsNullOrEmpty(defaultValue))
            {
                result = defaultValue;
            }
            else
            {
                if (!returnEmptyIfNotFound)
                    result = resourceKey;
            }

            return result;
        }

        #endregion
    }
}