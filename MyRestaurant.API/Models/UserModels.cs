using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.API.Models
{
    public class UserModels
    {
        public string UserId { get; set; }
        public string UserName {get;set;}
        public string Password { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /*[Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(15)]
        public string Login { get; set; }

        [Required]
        [MaxLength(34)]
        public string Password { get; set; }

        [Required]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(25)]
        public string LastName { get; set; }

        public string Avatar { get; set; }*/
    }
}
