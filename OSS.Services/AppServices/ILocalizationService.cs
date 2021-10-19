using OSS.Data.Entities;
using System.Collections.Generic;


namespace OSS.Services.AppServices
{
    public partial interface ILocalizationService
    {
        public int WorkingLanguageId { get; set; }
        void DeleteLocaleStringResource(LocaleStringResource localeStringResource);
        LocaleStringResource GetLocaleStringResourceById(int localeStringResourceId);
        LocaleStringResource GetLocaleStringResourceByName(string resourceName);
        LocaleStringResource GetLocaleStringResourceByName(string resourceName, int languageId,
            bool logIfNotFound = true);

        IList<LocaleStringResource> GetAllResources(int languageId);
        void InsertLocaleStringResource(LocaleStringResource localeStringResource);
        void UpdateLocaleStringResource(LocaleStringResource localeStringResource);
        Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId, bool? loadPublicLocales);

        string GetResource(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);

    }
}