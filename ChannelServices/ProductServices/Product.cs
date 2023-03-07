using ChannelServices.ChannelServices;
using ChannelServices.CategoriesServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ProductServices
{
    public class Product
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        //if product be for a channel, this property not will be null
        public string ChannelId { get; set; }

        public List<Comment> Comments { get; set; }
        public Category Category { get; set; }

        [ForeignKey("ChannelId")]
        public Channel Channel { get; set; }

        public List<ProductImageInfo> ProductImages { get; set; }
    }
}
