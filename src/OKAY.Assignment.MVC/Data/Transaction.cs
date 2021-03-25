using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKAY.Assignment.MVC.Data
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [ForeignKey("Property")]
        public int propertyId { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid userId { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Property Property { get; set; }
    }
}
