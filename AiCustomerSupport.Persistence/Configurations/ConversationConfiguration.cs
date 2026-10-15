using AiCustomerSupport.Domain;
using AiCustomerSupport.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiCustomerSupport.Persistence.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            // Store the enum values for conversation status as strings
            // instead of numeric values for database readability.

            builder.Property(c => c.Status)
                .HasConversion<string>()
                .HasDefaultValue(ConversationStatus.Active);


            builder.Property(c => c.Title)
                .HasMaxLength(100);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
