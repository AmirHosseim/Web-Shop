using ChannelServices.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class ProductsListViewModel
    {
        public string SearhString { get; set; }

        public List<Product> Products { get; set; }
    }
}
