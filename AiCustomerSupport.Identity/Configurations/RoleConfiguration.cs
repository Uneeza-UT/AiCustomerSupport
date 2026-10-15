using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiCustomerSupport.Identity.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "a17f3c82-6b49-4e51-9d20-8f73b1c604a5",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },

                new IdentityRole
                {
                    Id = "b28e4d93-7c50-4f62-ae31-9a84c2d715b",
                    Name = "Agent",
                    NormalizedName = "AGENT"
                },

                new IdentityRole
                {
                    Id = "c39f5ea4-8d61-4073-bf42-ab95d3e826c7",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                }
            );
        }
    }
}
