using Microsoft.AspNetCore.Http;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OSS.Web.Framework.Middleware
{
    public class OSSStatusCodeMiddleware : IMiddleware
    {
        private readonly ILogService _logger;
        private readonly IWebHelper _webHelper;

        public OSSStatusCodeMiddleware(ILogService logger, IWebHelper webHelper)
        {
            _logger = logger;
            _webHelper = webHelper;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);

            if (context.Response.HasStarted)
                return;

            int statusCode = context.Response.StatusCode;

            if (statusCode == StatusCodes.Status404NotFound)
            {
                if (!_webHelper.IsStaticResource())
                {
                    var originalPath = context.Request.Path;
                    var originalQueryString = context.Request.QueryString;
                    string userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

                    var m = new LogModel
                    {
                        LogLevelId = (int)LogLevel.Error,
                        ShortMessage = $"Error 404. The requested page ({originalPath}) was not found",
                        UserId = userId,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        PageUrl = _webHelper.GetThisPageUrl(true),
                        ReferrerUrl = _webHelper.GetUrlReferrer()
                    };
                    await _logger.Insert(m);

                    context.Response.Redirect(context.Request.PathBase + "/Home/PageNotFound");
                }
            }
            else if (statusCode == StatusCodes.Status400BadRequest)
            {
                string userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var m = new LogModel
                {
                    LogLevelId = (int)LogLevel.Error,
                    ShortMessage = "Error 400. Bad request",
                    UserId = userId,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    PageUrl = _webHelper.GetThisPageUrl(true),
                    ReferrerUrl = _webHelper.GetUrlReferrer()
                };
                await _logger.Insert(m);
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
