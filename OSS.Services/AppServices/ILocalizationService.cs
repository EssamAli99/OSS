using OSS.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    public partial interface ILocalizationService
    {
        public int WorkingLanguageId { get; set; }
        Task DeleteLocaleStringResource(LocaleStringResource localeStringResource);
        Task<LocaleStringResource> GetLocaleStringResourceById(int localeStringResourceId);
        Task<LocaleStringResource> GetLocaleStringResourceByName(string resourceName);
        Task<LocaleStringResource> GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true);

        Task<IList<LocaleStringResource>> GetAllResources(int languageId);
        Task InsertLocaleStringResource(LocaleStringResource localeStringResource);
        Task UpdateLocaleStringResource(LocaleStringResource localeStringResource);
        Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId, bool? loadPublicLocales);

        string GetResource(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);

    }
}