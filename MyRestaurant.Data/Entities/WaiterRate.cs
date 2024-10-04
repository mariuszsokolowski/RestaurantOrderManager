using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Entities
{
    public class WaiterRate
    {
        [Key]
        public int WaiterRateID { get; set; }

        public string WaiterID { get; set; }
        
        [NotMapped]
        public virtual Data.Entities.User Waiter { get; set; }

        [Required]
        public Enums.RatingType Type { get; set; }

        [Required]
        public int NotificationID { get; set; }

        public virtual Notification Notification { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        [Range(1, 5)]
        public decimal Rating { get; set; }

        public bool Cancled { get; set; }

        public string UserSign { get; set; }
    }
}
