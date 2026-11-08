using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFramework4
{
    public class CustomerAccount
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string AccountNumber { get; set; }
        public Account Account { get; set; }

        public string OwnershipType { get; set; }
        public DateTime OwnershipStartDate { get; set; }
        public string AccountStatus { get; set; }
    }
}
