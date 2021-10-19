using OSS.Services.AppServices;
using OSS.Web.Framework;
using OSS.Web.Localization;

namespace OSS.Web.Mvc.Razor
{
    /// <summary>
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class BaseRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        private ILocalizationService _localizationService;
        private IWorkContext _workContext;
        private Localizer _localizer;

        /// <summary>
        /// Get a localized resources
        /// </summary>
        public Localizer T
        {
            get
            {
                if (_localizationService == null)
                {
                    var con = Context.RequestServices.GetService(typeof(ILocalizationService));
                    _localizationService = con as ILocalizationService;
                }
                if (_workContext == null)
                {
                    var con = Context.RequestServices.GetService(typeof(IWorkContext));
                    _workContext = con as IWorkContext;
                }
                int langId = _workContext.WorkingLanguageId;
                if (_localizer == null)
                {
                    _localizer = (format, args) =>
                    {
                        var resFormat = _localizationService.GetResource(format, langId);
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return new LocalizedString((args == null || args.Length == 0)
                            ? resFormat
                            : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }
    }

    /// <summary>
    /// Web view page
    /// </summary>
    public abstract class BaseRazorPage : BaseRazorPage<dynamic>
    {
    }
}