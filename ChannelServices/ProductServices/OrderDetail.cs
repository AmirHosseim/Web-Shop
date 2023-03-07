using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ProductServices
{
    public class OrderDetail
    {
        [Key]
        public long Id { get; set; }

        public string ProductId { get; set; }

        public long OrderId { get; set; }

        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }


        public Product product { get; set; }

        public Order order { get; set; }
    }
}
