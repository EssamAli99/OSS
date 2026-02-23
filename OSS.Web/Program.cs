using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OSS.Data;
using OSS.Web;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var environment = builder.Environment;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
// Obsolete: IActionContextAccessor is planned for removal. Using it only for legacy support if needed.
builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

var strConn = configuration.GetConnectionString("DefaultConnection");
strConn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\MonaA\source\repos\EssamAli99\OSS\OSS.Web\App_Data\OSS_DB.mdf;Integrated Security=True";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(strConn));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

var keysFolder = Path.Combine(environment.ContentRootPath, "ProtectionKeys");
builder.Services.AddDataProtection()
    .SetApplicationName("Oss")
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetDefaultKeyLifetime(TimeSpan.FromDays(30));

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

OSS.Web.ServiceCollectionExtensions.AddDependencyInjection(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
OSSApplicationBuilderExtensions.UseOSSExceptionHandler(app, environment);
OSSApplicationBuilderExtensions.UseBadRequestResult(app);
OSSApplicationBuilderExtensions.UsePageNotFound(app);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(context =>
{
    if (context.HttpContext.Response.StatusCode == 403)
    {
        context.HttpContext.Response.Redirect("/Identity/Account/AccessDenied");
    }
    return Task.CompletedTask;
});

app.MapControllerRoute(name: "areaRoute", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapControllerRoute(name: "AccessDenied", pattern: "/Identity/Account/AccessDenied");

OSSApplicationBuilderExtensions.StartScheduleTasks(app);

app.Run();
