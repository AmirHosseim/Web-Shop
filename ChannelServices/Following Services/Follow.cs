using ChannelServices.ChannelServices;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.Following_Services
{
    public class Follow
    {
        public string UserId { get; set; }

        public string ChannelId { get; set; }

        public Channel channel { get; set; }

        public IdentityUser User { get; set; }
    }
}
