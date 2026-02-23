using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using NSubstitute;
using OSS.Data;
using OSS.Services.AppServices;
using OSS.Services.DomainServices;
using OSS.Services.Models;
using OSS.Web.Framework;
using OSS.Web.Validators;
using System;
using System.IO;
using System.Linq;

namespace OSS.Services.Tests
{
    public class OSSBaseTest
    {
        private static readonly ServiceProvider _serviceProvider;
        static OSSBaseTest()
        {
            var services = new ServiceCollection();

            services.AddHttpClient();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            var hostApplicationLifetime = Substitute.For<IHostApplicationLifetime>();
            services.AddSingleton(hostApplicationLifetime);

            var rootPath =
                new DirectoryInfo(
                        $@"{Directory.GetCurrentDirectory().Split("bin")[0]}{Path.Combine(@"\..\..\OSS.Web".Split('\\', '/').ToArray())}")
                    .FullName;

            var webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            webHostEnvironment.WebRootPath.Returns(Path.Combine(rootPath, "wwwroot"));
            webHostEnvironment.ContentRootPath.Returns(rootPath);
            webHostEnvironment.EnvironmentName.Returns("test");
            webHostEnvironment.ApplicationName.Returns("oss");
            services.AddSingleton(webHostEnvironment);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Host = "127.0.0.1";

            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(httpContext);

            services.AddSingleton(httpContextAccessor);

            var actionContextAccessor = Substitute.For<IActionContextAccessor>();
            actionContextAccessor.ActionContext
                .Returns(new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor()));

            services.AddSingleton(actionContextAccessor);

            services.AddTransient(provider => actionContextAccessor);

            var tempDataDictionaryFactory = Substitute.For<ITempDataDictionaryFactory>();
            var dataDictionary = new TempDataDictionary(httpContextAccessor.HttpContext!,
                Substitute.For<ITempDataProvider>());
            tempDataDictionaryFactory.GetTempData(Arg.Any<HttpContext>()).Returns(dataDictionary);
            services.AddSingleton(tempDataDictionaryFactory);

            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddSingleton<IWorkContext, WorkContext>();
            services.AddSingleton<IWebHelper, WebHelper>();
            services.AddSingleton<ILocker, MemoryCacheManager>();
            services.AddTransient(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddTransient<IValidator<TestTableModel>, TestTableValidator>();
            services.AddTransient<ITestTableService, TestTableService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<ILogService, DefaultLogService>();
            services.AddTransient<IAppPageService, AppPageService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IEmailAccountService, EmailAccountService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmtpBuilder, SmtpBuilder>();
            services.AddTransient<IOSSFileProvider, OSSFileProvider>();
            services.AddTransient<IScheduleTaskService, ScheduleTaskService>();

            _serviceProvider = services.BuildServiceProvider();

        }
        public T GetService<T>() where T : notnull
        {
            try
            {
                return _serviceProvider.GetRequiredService<T>();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
