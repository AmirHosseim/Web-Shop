using ChannelServices.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class AddRateViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int RatePoan { get; set; }

        public string ProductId { get; set; }

        public List<RatePoan> RatePoans { get; set; }
    }
}
