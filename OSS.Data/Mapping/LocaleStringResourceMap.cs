using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSS.Data.Entities;

namespace OSS.Data.Mapping
{
    public class LocaleStringResourceMap : IEntityTypeConfiguration<LocaleStringResource>
    {
        public void Configure(EntityTypeBuilder<LocaleStringResource> builder)
        {
            builder.ToTable(nameof(LocaleStringResource));
        }
    }
}
