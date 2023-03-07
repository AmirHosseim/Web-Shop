using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ProductServices
{
    public class SettelmentReceipt
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public decimal Value { get; set; }

        public long OrderId { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public Order Order { get; set; }
    }
}
