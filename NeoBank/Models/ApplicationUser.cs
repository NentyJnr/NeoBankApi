using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace NeoBank.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } 

        public string LastName { get; set; } 

        public DateTime DateOfBirth { get; set; }

        public DateTime DateCreated { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
}
