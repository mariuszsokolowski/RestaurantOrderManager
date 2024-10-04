using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MyRestaurant.Data.Entities
{


    public partial class User : IdentityUser
    {
      //  public string Id { get; set; }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsBlocked { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual string Name { get
            {
                return FirstName + " " + LastName;
            } }


    
    }

    public partial class UserRole : IdentityUserRole<string>
    {
        [JsonIgnore]
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public partial class Role : IdentityRole
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
