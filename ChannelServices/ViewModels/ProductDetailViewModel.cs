using ChannelServices.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }

        public List<Product> SameProducts { get; set; }

    }
}
