using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ProductServices
{
    public class Order
    {
        [Key]
        public long Id { get; set; }

        public string UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public bool IsFinished { get; set; }


        public List<OrderDetail> Orderdetails { get; set; }

        public SettelmentReceipt SettelmentReceipt { get; set; }
    }
}
