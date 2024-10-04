using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Entities
{
    public class OrderRate
    {
        [Key]
        public int RateID {get;set;}

        [Required]
        public Enums.RatingType Type { get; set; }

        [Required]
        public int OrderLineId { get; set; }

        public virtual OrderLine OrderLine { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        [Range(1,5)]
        public decimal Rating { get; set; }

        public bool Cancled { get; set; }

        public string UserSign { get; set; }

    }
}
