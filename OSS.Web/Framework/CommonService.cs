using OSS.Services.DomainServices;
using OSS.Services.Models;
using OSS.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Framework
{
    public class CommonService : ICommonService
    {
        private readonly IAppPageService _appPageService;
        private readonly ILanguageService _languageService;
        public CommonService(IAppPageService appPageService, ILanguageService languageService)
        {
            _appPageService = appPageService;
            _languageService = languageService;
        }
        public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModel(int currentLangId)
        {
            var availableLanguages = await _languageService.GetLanguagesAsync();

            return new LanguageSelectorModel
            {
                CurrentLanguageId = currentLangId,
                AvailableLanguages = availableLanguages
            };

        }
        public async Task<SiteMapNode> PrepareSideMenu(string allowedPaged)
        {
            var result = new SiteMapNode();
            var allPages = await _appPageService.GetAppPagesAsync(allowedPaged);
            var parents = allPages.Where(x => x.AppPageId == null).OrderBy(a => a.PageOrder).ToList();
            foreach (var p in parents)
            {
                var node = PopulateNode(p);
                PopulateTree(ref node, ref allPages, p.Id);
                result.ChildNodes.Add(node);

            }

            return result;
        }

        #region Utilities

        private void PopulateTree(ref SiteMapNode result, ref List<AppPageModel> allPages, int PId)
        {
            var childern = allPages.Where(x => x.AppPageId == PId).ToList();
            if (childern != null && childern.Count > 0)
            {
                foreach (var c in childern)
                {
                    result.ChildNodes.Add(PopulateNode(c));
                    PopulateTree(ref result, ref allPages, c.Id);
                }

            }
        }
        private static SiteMapNode PopulateNode(AppPageModel ap)
        {
            var node = new SiteMapNode
            {
                Id = ap.Id,
                ActionName = ap.ActionName,
                ControllerName = ap.ControllerName,
                IconClass = ap.IconClass,
                OpenUrlInNewTab = false,
                SystemName = ap.SystemName,
                Title = ap.Title,
                Url = "",
                Visible = true,
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary { { "area", ap.AreaName } }
            };
            return node;
        }
        #endregion
    }
}
