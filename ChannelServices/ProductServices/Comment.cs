using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ProductServices
{
    public class Comment
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int RatePoan { get; set; }
        
        public string UserId { get; set; }

        public string ProductId { get; set; }

        public IdentityUser User { get; set; }

        public Product Product { get; set; }
        public List<RatePoan> RatePoans { get; set; }
    }

    public class RatePoan
    {
        public int Id { get; set; }

        public int Rate { get; set; }

        public string TiTle { get; set; }
    }
}
