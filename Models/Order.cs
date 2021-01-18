using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Musebox_Web_Project.Models
{
    public class Order
    {
        [Key]
        [Required]
        [Display(Name = "Order ID")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Username")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

    }
}
