using System;
using System.Collections.Generic;
using System.Text;

namespace RMQ.Domain
{
    public class PurchaseTransaction
    {
        public int Id { get; set; }
        public decimal AmountToPay { get; set; }
        public string PurchaseNumber { get; set; }

        public string CompanyName { get; set; }
       
        public int CardTypeId { get; set; }
    }
}
