using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ProductServices
{
    public class ProductImageInfo
    {
        [Key]
        public string ImageId { get; set; }

        public string ProductId { get; set; }

        public string FileName { get; set; }

        public string Format { get; set; }

        public Product Product { get; set; }
    }
}
