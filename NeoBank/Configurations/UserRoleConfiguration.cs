using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NeoBank.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    // Admin
                    UserId = "7f8df141-8a3e-4f3f-82d3-0a89626a4b1c",
                    RoleId = "a4f78d09-86e3-4e96-a91b-3713e8043c7c"
                },
                new IdentityUserRole<string>
                {
                    // User
                    UserId = "b25a925a-9fbd-4e49-89f1-8ec446a8f023",
                    RoleId = "e3f7a8c1-b55c-4e4e-8893-89e440da1bbd"
                }
            );
        }
    }
}
