using System;
using System.ComponentModel.DataAnnotations;

namespace OKAY.Assignment.MVC.Models
{
    public class TransactionViewModel
    {
        public int id { get; set; }
        public Guid userId { get; set; }
        public int propertyId { get; set; }
        [Display(Name = "Owner")]
        public string userName { get; set; }
        [Display(Name = "Property Name")]
        public string propertyName { get; set; }
        [Display(Name = "Transaction Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime TransactionDate { get; set; }
    }
}
