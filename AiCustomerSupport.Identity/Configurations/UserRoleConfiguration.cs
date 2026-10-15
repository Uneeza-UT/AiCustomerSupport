using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiCustomerSupport.Identity.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    UserId = "d40a6fb5-9e72-4184-8053-bca6e4f937d8",
                    RoleId = "a17f3c82-6b49-4e51-9d20-8f73b1c604a5" // Admin.
                },

                new IdentityUserRole<string>
                {
                    UserId = "e51b70c6-af83-4295-9164-cdb7f50848e9",
                    RoleId = "b28e4d93-7c50-4f62-ae31-9a84c2d715b" // Agent.
                },

                new IdentityUserRole<string>
                {
                    UserId = "f62c81d7-b094-43a6-a275-dec8061959fa",
                    RoleId = "c39f5ea4-8d61-4073-bf42-ab95d3e826c7" // Customer.
                }
            );
        }
    }
}
