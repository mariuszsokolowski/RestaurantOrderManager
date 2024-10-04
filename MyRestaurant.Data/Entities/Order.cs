using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRestaurant.Data.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public float DocTotal { get; set; }

        [Required]
        public string UserSignId { get; set; }
        

        [NotMapped]
        [ForeignKey("UserSignId")]
        public virtual User UserSign { get; set; }

        [Required]
        public Data.Enums.PaymentMethod PaymentMethod { get; set; }
        //public virtual User UserSign { get; set; }


        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public bool Close { get; set; }

        public bool Cancel { get; set; }

        public ICollection<OrderLine> OrderLines { get; set; }
    }
}
