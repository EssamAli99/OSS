using OSS.Web.Models;
using System.Threading.Tasks;

namespace OSS.Web.Framework
{
    public interface ICommonService
    {
        Task<SiteMapNode> PrepareSideMenu(string allowedPaged);
        Task<LanguageSelectorModel> PrepareLanguageSelectorModel(int currentLangId);
    }
}
