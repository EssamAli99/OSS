using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OSS.Web.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace OSS.Web.Controllers
{
    [Authorize]
    [AuthorizeAction]
    public class BaseController : Controller
    {
        protected Dictionary<string, string> GetParameters()
        {
            var q = Request.Query; // if called with get
            var f = Request.Form; // if called with post
            //IEnumerable<KeyValuePair<string, string>> param = null;
            if (q.Any())
            {
                return q.ToDictionary(x => x.Key, x=> x.Value.ToString());
                //param = q.Select(x =>
                //{
                //    return new KeyValuePair<string, string>(x.Key, x.Value.ToString());
                //});
            }
            else
            {
                if (f.Any())
                    return f.ToDictionary(x => x.Key, x => x.Value.ToString());
                //param = f.Select(x =>
                //    {
                //        return new KeyValuePair<string, string>(x.Key, x.Value.ToString());
                //    });
            }
            return null;
        }
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
