namespace OSS.Services
{
    public enum ModelActions
    {
        List,
        Add,
        Edit,
        Delete
    }

    /// <summary>
    /// Represents priority of queued email
    /// </summary>
    public enum QueuedEmailPriority
    {
        /// <summary>
        /// Low
        /// </summary>
        Low = 0,

        /// <summary>
        /// High
        /// </summary>
        High = 5
    }

    /// <summary>
    /// Represents default values related to localization services
    /// </summary>
    public static partial class OSSDefaults
    {
        #region Languages

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LanguagesByIdCacheKey => "OSS.language.id-{0}";

        public static string LanguagesAllCacheKey => "OSS.language.all";

        public static string ApplicationLanguageCacheKey => "OSS.Lang";
        public static bool LoadAllLocaleRecordsOnStartup => true;

        public static int CacheTime => 60;

        #endregion

        #region Locales

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesAllPublicCacheKey => "OSS.lsr.all.public-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesAllCacheKey => "OSS.lsr.all-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public static string LocaleStringResourcesAllAdminCacheKey => "OSS.lsr.all.admin-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : resource key
        /// </remarks>
        public static string LocaleStringResourcesByResourceNameCacheKey => "OSS.lsr.{0}-{1}";

        /// <summary>
        /// Gets a key pattern for local string resources
        /// </summary>
        public static string LocaleStringResourcesPatternCacheKey => "OSS.lsr.";
        /// <summary>
        /// Gets a key pattern for application pages
        /// </summary>
        public static string ApplicationPagesCacheKey => "OSS.App.all.pg";

        /// <summary>
        /// Gets a prefix of locale resources for the admin area
        /// </summary>
        public static string AdminLocaleStringResourcesPrefix => "OSS.Admin.";

        /// <summary>
        /// Gets a prefix of locale resources for enumerations 
        /// </summary>
        public static string EnumLocaleStringResourcesPrefix => "OSS.Enums.";

        /// <summary>
        /// Gets a prefix of locale resources for permissions 
        /// </summary>
        public static string PermissionLocaleStringResourcesPrefix => "OSS.Permission.";

        #endregion

        #region Localized properties

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : entity ID
        /// {2} : locale key group
        /// {3} : locale key
        /// </remarks>
        public static string LocalizedPropertyCacheKey => "OSS.localizedproperty.value-{0}-{1}-{2}-{3}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string LocalizedPropertyAllCacheKey => "OSS.localizedproperty.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string LocalizedPropertyPatternCacheKey => "OSS.localizedproperty.";

        #endregion

        #region Http Defaults
        /// <summary>
        /// Gets the name of the default HTTP client
        /// </summary>
        public static string DefaultHttpClient => "default";

        /// <summary>
        /// Gets the name of HTTP_CLUSTER_HTTPS header
        /// </summary>
        public static string HttpClusterHttpsHeader => "HTTP_CLUSTER_HTTPS";

        /// <summary>
        /// Gets the name of HTTP_X_FORWARDED_PROTO header
        /// </summary>
        public static string HttpXForwardedProtoHeader => "X-Forwarded-Proto";

        #endregion
    }
}