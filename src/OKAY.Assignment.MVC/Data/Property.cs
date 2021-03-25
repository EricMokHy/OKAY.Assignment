using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKAY.Assignment.MVC.Data
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [MaxLength(200)]
        [Required]
        public string name { get; set; }
        public int bedroom { get; set; }
        [Required]
        public bool isAvailable { get; set; }
        [Required]
        [DataType("decimal(10, 2)")]
        public decimal leasePrice { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid userId { get; set; }
        [Required]
        public DateTime createdDate { get; set; }
        [Required]
        public DateTime updatedDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
