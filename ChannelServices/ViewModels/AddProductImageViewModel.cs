using ChannelServices.ProductServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ChannelServices.ViewModels
{
    public class AddProductImageViewModel
    {
        public string ProductId { get; set; }

        public List<ProductImageInfo> AddedImages { get; set; }

        public IFormFile Image { get; set; }

        public bool IsAllow { get; set; }
    }
}
