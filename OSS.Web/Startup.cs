using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSS.Data;

//using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using OSS.Services;
using OSS.Services.Models;
using System;

namespace OSS.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //    //options.MinimumSameSitePolicy = SameSiteMode.Unspecified;

            //});
            var scon = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Working\CODE\OSS\OSS.Web\OSS.Web\App_Data\OSS_DB.mdf;Integrated Security=True";
            var constr = Configuration.GetConnectionString("DefaultConnection");
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(scon));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //.AddDefaultTokenProviders()
                ;

            // to setup login page and access denied page and cookie time
            // this configuration can be removed but there will not be default login page
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddDataProtection()
                .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

            services.AddMvc(opt => opt.EnableEndpointRouting = false)
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null)
                .AddFluentValidation()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            //create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(AllMapperConfiguration));
                AutoMapper.Internal.InternalApi.Internal(cfg).ForAllMaps((mapConfiguration, map) =>
                {
                    if (typeof(BaseModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    {
                        map.ForMember(nameof(BaseModel.ModelMode), options => options.Ignore());
                        map.ForMember(nameof(BaseModel.EncrypedId), options => options.MapFrom(entity => ((BaseEntity)entity).Id.ToString()));
                    }

                    if (typeof(BaseEntity).IsAssignableFrom(mapConfiguration.DestinationType))
                    {
                        map.ForMember(nameof(BaseEntity.Id), options => options.MapFrom(entity => int.Parse(((BaseModel)entity).EncrypedId)));
                    }
                });
            });

            //register
            AutoMapperConfiguration.Init(config);

            ApplicationBuilderExtensions.AddDependencyInjection(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            ApplicationBuilderExtensions.UseOSSExceptionHandler(app, env);
            ApplicationBuilderExtensions.UseBadRequestResult(app);
            ApplicationBuilderExtensions.UsePageNotFound(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages(async context =>
            {
                if (context.HttpContext.Response.StatusCode == 403)
                {
                    context.HttpContext.Response.Redirect("/Identity/Account/AccessDenied");
                }
            });


            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(name: "areaRoute",
                        pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");
                    endpoints.MapControllerRoute(name: "default",
                        pattern: $"{{controller=Home}}/{{action=Index}}/{{id?}}");
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapRazorPages(); // map identity pages
                    endpoints.MapControllerRoute(name: "AccessDenied", pattern: "/Identity/Account/AccessDenied");
                });

            ApplicationBuilderExtensions.StartScheduleTasks(app);
            
        }
    }
}
