using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSS.Data.Entities;

namespace OSS.Data.Mapping
{
    public class AppPageMap : IEntityTypeConfiguration<AppPage>
    {
        public void Configure(EntityTypeBuilder<AppPage> builder)
        {
            builder.ToTable(nameof(AppPage));
            builder.HasKey(a => a.Id);
            builder.HasMany(a => a.ChildNodes)
                .WithOne()
                .HasForeignKey(a=> a.AppPageId)
                .IsRequired(false);
        }
    }
}
