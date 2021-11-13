
using AutoMapper;
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
using Moq;
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

            //services.AddHttpContextAccessor();
            //services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(scon));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //.AddDefaultTokenProviders()
                ;

            ////add configuration parameters
            //var appSettings = new AppSettings();
            //services.AddSingleton(appSettings);
            //Singleton<AppSettings>.Instance = appSettings;

            var hostApplicationLifetime = new Mock<IHostApplicationLifetime>();
            services.AddSingleton(hostApplicationLifetime.Object);

            var rootPath =
                new DirectoryInfo(
                        $@"{Directory.GetCurrentDirectory().Split("bin")[0]}{Path.Combine(@"\..\..\OSS.Web".Split('\\', '/').ToArray())}")
                    .FullName;

            //Presentation\Nop.Web\wwwroot
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(p => p.WebRootPath).Returns(Path.Combine(rootPath, "wwwroot"));
            webHostEnvironment.Setup(p => p.ContentRootPath).Returns(rootPath);
            webHostEnvironment.Setup(p => p.EnvironmentName).Returns("test");
            webHostEnvironment.Setup(p => p.ApplicationName).Returns("oss");
            services.AddSingleton(webHostEnvironment.Object);

            var httpContext = new DefaultHttpContext
            {
                Request = { Headers = { { HeaderNames.Host, "127.0.0.1" } } }
            };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(p => p.HttpContext).Returns(httpContext);

            services.AddSingleton(httpContextAccessor.Object);

            var actionContextAccessor = new Mock<IActionContextAccessor>();
            actionContextAccessor.Setup(x => x.ActionContext)
                .Returns(new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor()));

            services.AddSingleton(actionContextAccessor.Object);

            services.AddTransient(provider => actionContextAccessor.Object);

            var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            var dataDictionary = new TempDataDictionary(httpContextAccessor.Object.HttpContext,
                new Mock<ITempDataProvider>().Object);
            tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(dataDictionary);
            services.AddSingleton(tempDataDictionaryFactory.Object);

            //create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(AllMapperConfiguration));
            });

            //register
            AutoMapperConfiguration.Init(config);

            services.AddSingleton<ICacheManager, MemoryCacheManager>();
            services.AddSingleton<IWorkContext, WorkContext>();
            services.AddSingleton<IWebHelper, WebHelper>();
            services.AddSingleton<ILocker, MemoryCacheManager>();
            services.AddTransient(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddTransient<IValidator<TestTableModel>, TestTableValidator>();
            services.AddTransient<ITestTableService, TestTableService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<ILogger, DefaultLogger>();
            services.AddTransient<IAppPageService, AppPageService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IEmailAccountService, EmailAccountService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmtpBuilder, SmtpBuilder>();
            services.AddTransient<IOSSFileProvider, OSSFileProvider>();
            services.AddTransient<IScheduleTaskService, ScheduleTaskService>();


            _serviceProvider = services.BuildServiceProvider();

        }
        public T GetService<T>()
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
