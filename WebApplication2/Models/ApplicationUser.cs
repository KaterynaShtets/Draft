
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;

namespace Tut20.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Config { get; set; }
        public string Lang { get; set; }
        public bool OpenMode { get; set; }
        public string Ad { get; set; }
    }
}
