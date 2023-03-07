using ChannelServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class CategoryProductsViewModel
    {
        public ShowProductsViewModel showProduct { get; set; }

        public string CategoryName { get; set; }

        public int CategoryId { get; set; }
    }
}
