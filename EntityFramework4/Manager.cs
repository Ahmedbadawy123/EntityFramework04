using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework4
{
    public class Manager
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public string Address { get; set; }

        public string BranchCode { get; set; }
        public Branch Branch { get; set; }
    }
}
