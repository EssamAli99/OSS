using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSS.Data.Entities;

namespace OSS.Data.Mapping
{
    public class PersonMap : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable(nameof(Person));
            //builder.HasKey(a => a.UserId);
            //builder.HasMany(a => a.UserLogs)
            //    .WithOne(a=> a.User)
            //    .HasForeignKey(a=> a.UserId)
            //    .IsRequired(false);
        }
    }
}
