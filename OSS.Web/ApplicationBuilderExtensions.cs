using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OSS.Services;
using OSS.Services.AppServices;
using OSS.Services.DomainServices;
using OSS.Services.Models;
using OSS.Web.Framework;
using OSS.Web.Validators;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OSS.Web
{
    public static class ApplicationBuilderExtensions
    {
        public static TaskManager TaskManagerInstance;

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        internal static void UseOSSExceptionHandler(IApplicationBuilder application, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                application.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return Task.CompletedTask;

                    try
                    {
                        //log error
                        string userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                        var scope = application.ApplicationServices.CreateScope();
                        var logger = scope.ServiceProvider.GetService<ILogger>();
                        var webHelper = scope.ServiceProvider.GetService<IWebHelper>();
                        var m = new LogModel
                        {
                            FullMessage = exception?.ToString()?? "",
                            LogLevelId = (int)LogLevel.Error,
                            ShortMessage = exception?.Message,
                            UserId = userId,
                            IpAddress = webHelper.GetCurrentIpAddress(),
                            PageUrl = webHelper.GetThisPageUrl(true),
                            ReferrerUrl = webHelper.GetUrlReferrer()
                        };
                        logger.Insert(m);
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }

                    return Task.CompletedTask;
                });
            });


        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        internal static void UsePageNotFound(IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 Not Found
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var scope = application.ApplicationServices.CreateScope();
                    var logger = scope.ServiceProvider.GetService<ILogger>();
                    var webHelper = scope.ServiceProvider.GetService<IWebHelper>();
                    string userId = context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                    if (!webHelper.IsStaticResource())
                    {
                        //get original path and query
                        var originalPath = context.HttpContext.Request.Path;
                        var originalQueryString = context.HttpContext.Request.QueryString;
                        var m = new LogModel
                        {
                            LogLevelId = (int)LogLevel.Error,
                            ShortMessage = $"Error 404. The requested page ({originalPath}) was not found",
                            UserId = userId,
                            IpAddress = webHelper.GetCurrentIpAddress(),
                            PageUrl = webHelper.GetThisPageUrl(true),
                            ReferrerUrl = webHelper.GetUrlReferrer()
                        };
                        logger.Insert(m);

                        try
                        {
                            //get new path
                            var pageNotFoundPath = "/Home/PageNotFound";
                            //re-execute request with new path
                            context.HttpContext.Response.Redirect(context.HttpContext.Request.PathBase + pageNotFoundPath);
                        }
                        finally
                        {
                            //return original path to request
                            context.HttpContext.Request.QueryString = originalQueryString;
                            context.HttpContext.Request.Path = originalPath;
                        }
                    }

                    await Task.CompletedTask;
                }
            });
        }

        internal static void StartScheduleTasks(IApplicationBuilder application)
        {
            if (TaskManagerInstance != null) return;
            var scope = application.ApplicationServices.CreateScope();
            var taskService = scope.ServiceProvider.GetService<IScheduleTaskService>();
            var client = scope.ServiceProvider.GetService<IHttpClientFactory>();
            var logger = scope.ServiceProvider.GetService<ILogger>();
            
            TaskManagerInstance = new TaskManager(taskService, client, logger);
            TaskManagerInstance.Initialize();
            TaskManagerInstance.Start();
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 400 status code (bad request)
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        internal static void UseBadRequestResult(IApplicationBuilder application)
        {
            application.UseStatusCodePages(context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var scope = application.ApplicationServices.CreateScope();
                    var logger = scope.ServiceProvider.GetService<ILogger>();
                    string userId = context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                    var webHelper = scope.ServiceProvider.GetService<IWebHelper>();
                    var m = new LogModel
                    {
                        LogLevelId = (int)LogLevel.Error,
                        ShortMessage = "Error 400. Bad request",
                        UserId = userId,
                        IpAddress = webHelper.GetCurrentIpAddress(),
                        PageUrl = webHelper.GetThisPageUrl(true),
                        ReferrerUrl = webHelper.GetUrlReferrer()
                    };
                    logger.Insert(m);
                }
                context.HttpContext.Response.Redirect("/Home/Error");
                return Task.CompletedTask;
            });
        }


        internal static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddSingleton<IWorkContext, WorkContext>();
            services.AddSingleton<IWebHelper, WebHelper>();
            services.AddSingleton<ILocker, MemoryCacheManager>();
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IValidator<TestTableModel>, TestTableValidator>();
            services.AddScoped<ITestTableService, TestTableService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ILogger, DefaultLogger>();
            services.AddScoped<IAppPageService, AppPageService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmtpBuilder, SmtpBuilder>();
            services.AddScoped<IOSSFileProvider, OSSFileProvider>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
        }
    }
}
