using Microsoft.AspNetCore.Mvc;
using OSS.Web.Framework;

namespace OSS.Web.Components
{
    public class LanguageSelector : ViewComponent
    {
        private readonly ICommonService _commonModelFactory;
        private readonly IWorkContext _workContext;

        public LanguageSelector(ICommonService commonModelFactory, IWorkContext workContext)
        {
            _commonModelFactory = commonModelFactory;
            _workContext = workContext;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareLanguageSelectorModel(_workContext.WorkingLanguageId);

            if (model.AvailableLanguages.Count == 1)
                return Content("");

            return View(model);
        }
    }
}
