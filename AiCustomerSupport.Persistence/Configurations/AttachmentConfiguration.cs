using AiCustomerSupport.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiCustomerSupport.Persistence.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.Property(a => a.FileName)
                .HasMaxLength(200);


            builder.Property(a => a.FilePath)
                .HasMaxLength(500);


            builder.Property(a => a.ContentType)
                .HasMaxLength(100);
        }
    }
}
