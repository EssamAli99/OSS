using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            //get action and controller names
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (!string.IsNullOrEmpty(actionName) && !string.IsNullOrEmpty(controllerName))
            {
                bool allowed = false;
                var p = context.HttpContext.User.Claims.Where(x => x.Type.Equals(controllerName)).FirstOrDefault();
                if (p != null)
                {
                    string id = OSSConfig.Permissions.Where(x => x.ActionName.Equals(actionName)).Select(x => x.Id).FirstOrDefault();
                    if (!string.IsNullOrEmpty(id) && p.Value.Contains(id)) allowed = true;
                }
                if (!allowed)
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.Result = new RedirectToActionResult("AccessDenied", "Common", new { area = "" });
                    return;
                }
            }
            //
        }
    }
}
