using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRestaurant.Data.Entities
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }



        [NotMapped]
        public virtual decimal Rating {get;set;}

        [Required]
        [Range(0.01,float.MaxValue)]
        public float Price { get; set; }

        public string Image { get; set; }

        public bool Promotion { get; set; }

        public bool Inactive { get; set; }

     
    }
}
