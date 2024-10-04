using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.API.Models
{
    public class OrderLineModels
    {

        
        [Key]
        public int Id { get; set; }

        //[Required]
        public Order Order { get; set; }

        


        //[Required]

        public Menu Menu { get; set; } 

        //[Required]
        public int MenuId { get; set; }

 

        // public Order OrderId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double Price { get; set; }

        public Data.Enums.PaymentMethod PaymentMethod { get; set; }

        public string UserSign { get; set; }
    }


}
