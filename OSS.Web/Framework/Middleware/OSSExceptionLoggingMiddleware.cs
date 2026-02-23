using Microsoft.AspNetCore.Http;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OSS.Web.Framework.Middleware
{
    public class OSSExceptionLoggingMiddleware : IMiddleware
    {
        private readonly ILogService _logger;
        private readonly IWebHelper _webHelper;

        public OSSExceptionLoggingMiddleware(ILogService logger, IWebHelper webHelper)
        {
            _logger = logger;
            _webHelper = webHelper;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                string userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var m = new LogModel
                {
                    FullMessage = exception.ToString(),
                    LogLevelId = (int)LogLevel.Error,
                    ShortMessage = exception.Message,
                    UserId = userId,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    PageUrl = _webHelper.GetThisPageUrl(true),
                    ReferrerUrl = _webHelper.GetUrlReferrer()
                };
                await _logger.Insert(m);
                throw;
            }
        }
    }
}
