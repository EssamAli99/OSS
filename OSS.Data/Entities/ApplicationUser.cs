using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSS.Data.Entities
{
    [NotMapped]
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            UserLogs = new HashSet<Log>();
        }
        //we can add other fields and use this on save
        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        //    // Add custom user claims here
        //    return userIdentity;
        //}

        public virtual ICollection<Log> UserLogs { get; set; }
    }
}
