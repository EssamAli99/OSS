using OSS.Services.Models;
using OSS.Web.Models;

namespace OSS.Web.Framework
{
    public interface ICommonService
    {
        SiteMapNode PrepareSideMenu(string allowedPaged);
        LanguageSelectorModel PrepareLanguageSelectorModel(int currentLangId);
    }
}
