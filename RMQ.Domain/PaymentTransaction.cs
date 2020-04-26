using System;

namespace RMQ.Domain
{
    [Serializable]
    public class PaymentTransaction
    {
        public int  Id { get; set; }
        public string  Name { get; set; }
        public string CardNumber { get; set; }
        public int CardTypeId { get; set; }

        public decimal AmountToPay { get; set; }

    }
}
