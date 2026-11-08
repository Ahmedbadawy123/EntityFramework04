using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EntityFramework4
{
    public class Branch
    {
        [Key]
        public string Code { get; set; } 
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public Manager Manager { get; set; }
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
