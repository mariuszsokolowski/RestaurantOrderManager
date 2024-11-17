using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantOrderManager.API.Models
{
    public sealed class LoginModels
    {
        [Required]
        public string Login { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
