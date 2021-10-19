using Microsoft.AspNetCore.Mvc;
using OSS.Web.Framework;

namespace OSS.Web.Controllers
{
    public class CommonController : Controller
    {
        private readonly IWorkContext _WorkContext;
        public CommonController(IWorkContext workContext)
        {
            _WorkContext = workContext;
        }
        public virtual IActionResult SetLanguage(int langid, string returnUrl = "")
        {
            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.Action("Index", "Home");

            _WorkContext.WorkingLanguageId = langid;
            return Redirect(returnUrl);
        }
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
