using Microsoft.AspNetCore.Mvc;
using OSS.Web.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Components
{
    public class SidebarMenu : ViewComponent
    {
        public readonly ICommonService _cModelFactory;
        public SidebarMenu(ICommonService cModelFactory)
        {
            _cModelFactory = cModelFactory;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
        {
            var allowedPages = "";
            if (this.UserClaimsPrincipal != null && this.UserClaimsPrincipal.Claims != null && this.UserClaimsPrincipal.Claims.Count () > 0)
            {
                allowedPages = this.UserClaimsPrincipal.Claims.Where(c=> c.Type == "AllowedPages").FirstOrDefault().Value;
            }

            var model = await _cModelFactory.PrepareSideMenu(allowedPages);

            return View(model);
        }
    }
}
