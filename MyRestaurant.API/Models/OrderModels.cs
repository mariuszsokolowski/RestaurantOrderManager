using MyRestaurant.Data;
using MyRestaurant.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace MyRestaurant.API.Models
{
    public class OrderModels
    {
       

        [Key]
        public int OrderId { get; set; }
        
     
        public float DocTotal { get; set; }

        
        public string UserSignId { get; set; }

      //  [ForeignKey("UserSignId")]
        //public virtual UserModels UserModels { get; set; }

       
        public DateTime CreateDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public bool Cancel { get; set; }

        public virtual ICollection<OrderLine> OrderLines { get; set; }

    }

    public class MainOrderModels
    {


       List<OrderLineModels> OrderLine { get; set; }


    }
}
