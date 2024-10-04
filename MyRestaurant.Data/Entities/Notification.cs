using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRestaurant.Data.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        public string UserSignId { get; set; }

        [NotMapped]
        public User UserSign { get; set; }

        public string UserAssigned { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? AssignedDate { get; set; }

        public DateTime? CloseDate { get; set; }
    }
}
