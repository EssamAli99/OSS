using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OSS.Data;
using OSS.Services;
using OSS.Services.AppServices;
using OSS.Services.DomainServices;
using OSS.Services.ExportImport;
using OSS.Services.Models;
using OSS.Web.Framework;
using OSS.Web.Validators;
using OSS.Data.Events;
using OSS.Services.Events;

namespace OSS.Web
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IPermissionProvider, PermissionProvider>();
            services.AddSingleton<MemoryCacheManager>();
            services.AddSingleton<ICacheManager>(sp => sp.GetRequiredService<MemoryCacheManager>());
            services.AddSingleton<ILocker>(sp => sp.GetRequiredService<MemoryCacheManager>());
            services.AddScoped<IWorkContext, WorkContext>();
            services.AddScoped<IWebHelper, WebHelper>();
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IValidator<TestTableModel>, TestTableValidator>();
            services.AddScoped<ITestTableService, TestTableService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<ILogService, DefaultLogService>();
            services.AddScoped<IAppPageService, AppPageService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IEmailAccountService, EmailAccountService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmtpBuilder, SmtpBuilder>();
            services.AddScoped<IOSSFileProvider, OSSFileProvider>();
            services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            services.AddScoped<IExportManager, ExportManager>();

            //events
            services.AddScoped<IEventPublisher, EventHandlerContainer>();
            services.AddScoped(typeof(IEventHandler<>), typeof(EntityEventHandler<>));
            services.AddScoped(typeof(EntityInsertedEvent<>), typeof(EntityEventHandler<>));
            services.AddScoped(typeof(EntityUpdatedEvent<>), typeof(EntityEventHandler<>));
            services.AddScoped(typeof(EntityDeletedEvent<>), typeof(EntityEventHandler<>));

            // middlewares
            services.AddScoped<OSS.Web.Framework.Middleware.OSSStatusCodeMiddleware>();
            services.AddScoped<OSS.Web.Framework.Middleware.OSSExceptionLoggingMiddleware>();
        }
    }
}
