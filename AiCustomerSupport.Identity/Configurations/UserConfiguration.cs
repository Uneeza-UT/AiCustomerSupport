using AiCustomerSupport.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiCustomerSupport.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            builder.HasData(
                new ApplicationUser
                {
                    Id = "d40a6fb5-9e72-4184-8053-bca6e4f937d8",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = "admin@example.com",
                    NormalizedUserName = "ADMIN@EXAMPLE.COM",
                    PasswordHash = passwordHasher.HashPassword(null, "Admin1234!"),
                    EmailConfirmed = true
                },

                new ApplicationUser
                {
                    Id = "e51b70c6-af83-4295-9164-cdb7f50848e9",
                    Email = "agent@example.com",
                    NormalizedEmail = "AGENT@EXAMPLE.COM",
                    FirstName = "Test",
                    LastName = "Agent",
                    UserName = "agent@example.com",
                    NormalizedUserName = "AGENT@EXAMPLE.COM",
                    PasswordHash = passwordHasher.HashPassword(null, "Agent1234!"),
                    EmailConfirmed = true
                },

                new ApplicationUser
                {
                    Id = "f62c81d7-b094-43a6-a275-dec8061959fa",
                    Email = "customer@example.com",
                    NormalizedEmail = "CUSTOMER@EXAMPLE.COM",
                    FirstName = "Test",
                    LastName = "Customer",
                    UserName = "customer@example.com",
                    NormalizedUserName = "CUSTOMER@EXAMPLE.COM",
                    PasswordHash = passwordHasher.HashPassword(null, "Customer1234!"),
                    EmailConfirmed = true
                }
            );
        }
    }
}
