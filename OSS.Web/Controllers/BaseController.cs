using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSS.Web.Mvc.Filters;

namespace OSS.Web.Controllers
{
    [Authorize]
    [AuthorizeAction]
    public class BaseController : Controller
    {
        //protected string PageName { 
        //    get 
        //    { 
        //        return HttpContext.Request.RouteValues["controller"].ToString(); 
        //    } 
        //}
        //protected bool IsPageAllowed
        //{
        //    get
        //    {
        //        var pageClaim = User.Claims.Where(x => x.Type.Equals(PageName)).FirstOrDefault();
        //        return (pageClaim != null);

        //        //// or
        //        //var p = User.Claims.Where(x => x.Type.Equals("AllowedPages")).FirstOrDefault();
        //        //if (p != null && p.Value.Contains(PageName)) return true;
        //        //return false;
        //    }
        //}
        //protected string[] AllowedPermissions
        //{
        //    get
        //    {
        //        var pageClaim = User.Claims.Where(x => x.Type.Equals(PageName)).FirstOrDefault();
        //        if (pageClaim != null)
        //        {
        //            return pageClaim.Value.Split(",");
        //        }
        //        return Array.Empty<string>();
        //    }
        //}

    }
}
