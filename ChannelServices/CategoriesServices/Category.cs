using ChannelServices.ProductServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.CategoriesServices
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Product> Products { get; set; }
    }
}
