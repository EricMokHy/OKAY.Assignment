using OKAY.Assignment.MVC.Data;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Identity
{
    public class ApplicationUser : IdentityUser<Guid> 
    {
        public ApplicationUser()
        {
            Properties = new HashSet<Property>();
            Transactions = new HashSet<Transaction>();
        }

        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }

    public class ApplicationRole : IdentityRole<Guid> 
    {
        public ApplicationRole(string Name)
        {
            base.Name = Name;
            
        }

        
    }
}
