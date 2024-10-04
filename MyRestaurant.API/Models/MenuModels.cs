using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyRestaurant.API.Models
{
    public class MenuModels
    {
      
            [Key]
            public int MenuId { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public string Description { get; set; }

            [Required]
            [Range(0.01, float.MaxValue)]
            public float Price { get; set; }

            public string Image { get; set; }

            public bool Promotion { get; set; }

            public IFormFile file { get; set; }

        
    }
}
