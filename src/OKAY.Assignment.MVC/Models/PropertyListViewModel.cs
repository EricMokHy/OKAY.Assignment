using System;
using System.ComponentModel.DataAnnotations;

namespace OKAY.Assignment.MVC.Models
{
    public class PropertyViewModel
    {
        public int id { get; set; }
        [Display(Name = "Name")]
        [Required]
        [MaxLength(200)]
        public string name { get; set; }
        [Display(Name = "Bedroom")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0}")]
        public int bedroom { get; set; }
        [Display(Name = "Is Available")]
        public bool isAvailable { get; set; }
        [Display(Name = "Lease Price")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal leasePrice { get; set; }
        [Display(Name = "Owner")]
        public string owner { get; set; }
        [Display(Name = "Created Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime createdDate { get; set; }
        [Display(Name = "Updated Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime updatedDate { get; set; }
    }
}
