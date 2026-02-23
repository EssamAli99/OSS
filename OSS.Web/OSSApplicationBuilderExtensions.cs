using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OSS.Data;
using OSS.Services;
using OSS.Services.AppServices;
using OSS.Services.DomainServices;
using OSS.Services.Events;
using OSS.Services.ExportImport;
using OSS.Services.Models;
using OSS.Web.Controllers;
using OSS.Web.Framework;
using OSS.Web.Validators;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OSS.Web
{
    public static class OSSApplicationBuilderExtensions
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

            //log errors via IMiddleware
            application.UseMiddleware<OSS.Web.Framework.Middleware.OSSExceptionLoggingMiddleware>();
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 404 status code that do not have a body
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        internal static void UsePageNotFound(IApplicationBuilder application)
        {
            application.UseMiddleware<OSS.Web.Framework.Middleware.OSSStatusCodeMiddleware>();
        }

        internal static void StartScheduleTasks(IApplicationBuilder application)
        {
            if (TaskManagerInstance != null) return;
            var scope = application.ApplicationServices.CreateScope();
            var taskService = scope.ServiceProvider.GetService<IScheduleTaskService>();
            var client = scope.ServiceProvider.GetService<IHttpClientFactory>();
            var logger = scope.ServiceProvider.GetService<ILogService>();

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
            // Functionality is now subsumed inherently by OSSStatusCodeMiddleware configured in UsePageNotFound.
        }



    }
}
