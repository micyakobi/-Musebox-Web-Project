using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Musebox_Web_Project.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Branch Address")]
        public string Address { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

    }
}
