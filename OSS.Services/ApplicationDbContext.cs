using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;

namespace OSS.Services
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<TestTable> TestTable { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<LocaleStringResource> LocaleStringResource { get; set; }
        public DbSet<AppPage> AppPage { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        public DbSet<ActivityLogType> ActivityLogType { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<AppPageAction> AppPageAction { get; set; }
        public DbSet<EmailAccount> EmailAccount { get; set; }
        public DbSet<QueuedEmail> QueuedEmail { get; set; }
        public DbSet<ScheduleTask> ScheduleTask { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    //// to add mapping
        //    //builder.ApplyConfiguration(new TestTableMap());
        //    //builder.ApplyConfiguration(new LanguageMap());
        //    //builder.ApplyConfiguration(new LocaleStringResourceMap());
        //    //builder.ApplyConfiguration(new AppPageMap());
        //    //builder.ApplyConfiguration(new LogMap());
        //    //builder.ApplyConfiguration(new PersonMap());
        //    //base.OnModelCreating(builder);
        //}

        //internal object AsNoTracking()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
