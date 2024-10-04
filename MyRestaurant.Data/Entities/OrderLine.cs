using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Entities
{
    public class OrderLine
    {
        [Key]
        public int OrderLineId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int MenuId { get; set; }

        public virtual Menu Menu { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double Price { get; set; }

   



    }
}
