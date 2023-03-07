using ChannelServices.ChannelServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.RecuritmentServices
{
    public class RecruitmentInChannelForm
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public string ChannelId { get; set; }

        public bool WasHired { get; set; }

        public Channel channel { get; set; }

        public List<RecruitmentInChannel> RecruitmentsInChannel { get; set; }

        public List<ReplyToRecruitmentForm> replysToRecruitmentForm { get; set; }
    }
}
