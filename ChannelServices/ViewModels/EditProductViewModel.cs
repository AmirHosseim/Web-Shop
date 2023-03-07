using ChannelServices.CategoriesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class EditProductViewModel
    { 
        public string Id { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string channelId { get; set; }

        public bool AllowToEditProduct { get; set; }

        public List<Category> Categories { get; set; }
    }
}
