using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoBank.Models;

namespace NeoBank.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            builder.HasData(
                new ApplicationUser()
                {
                    Id = "7f8df141-8a3e-4f3f-82d3-0a89626a4b1c",
                    Email = "admin@localhost.com",
                    NormalizedEmail = "ADMIN@LOCALHOST.COM",
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = "admin@localhost.com",
                    NormalizedUserName = "ADMIN@LOCALHOST.COM",
                    PasswordHash = hasher.HashPassword(null, "Password@1"),
                    EmailConfirmed = true,
                    DateCreated = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                },
                new ApplicationUser()
                {
                    Id = "b25a925a-9fbd-4e49-89f1-8ec446a8f023",
                    Email = "user@localhost.com",
                    NormalizedEmail = "USER@LOCALHOST.COM",
                    FirstName = "System",
                    LastName = "USER",
                    UserName = "user@localhost.com",
                    NormalizedUserName = "USER@LOCALHOST.COM",
                    PasswordHash = hasher.HashPassword(null, "Password@1"),
                    EmailConfirmed = true,
                    DateCreated = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                }
            );
        }
    }
}
