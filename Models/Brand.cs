using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Musebox_Web_Project.Models
{
    public class Brand
    {
        [Required]
        [DataType(DataType.Text)]
        [Key]
        [Display(Name = "Brand Name")]
        public int BrandId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }

    }
}
