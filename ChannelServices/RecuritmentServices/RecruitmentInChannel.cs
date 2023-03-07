using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChannelServices.ChannelServices;
using ChannelServices.RecuritmentServices;
using Microsoft.AspNetCore.Identity;

namespace ChannelServices.RecuritmentServices
{
    public class RecruitmentInChannel
    {
        public string UserId { get; set; }

        public string ChannelId { get; set; }

        public int RecuitmentFormId { get; set; }

        public Channel channel { get; set; }

        public IdentityUser User { get; set; }
    }
}
