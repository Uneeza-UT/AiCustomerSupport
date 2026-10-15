using AiCustomerSupport.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiCustomerSupport.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(q => q.Message)
                .HasMaxLength(500);

            builder.Property(q => q.IsRead)
                .HasDefaultValue(false);
        }
    }
}
