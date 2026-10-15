using AiCustomerSupport.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiCustomerSupport.Persistence.Configurations
{
    public class IntentConfiguration : IEntityTypeConfiguration<Intent>
    {
        public void Configure(EntityTypeBuilder<Intent> builder)
        {
            builder.Property(i => i.Name)
                .HasMaxLength(100);
        }
    }
}
