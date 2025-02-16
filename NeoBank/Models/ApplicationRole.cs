using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace NeoBank.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Name { get; set; }
        public string RoleDescription { get; set; }
        public DateTime DateCreated { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}
