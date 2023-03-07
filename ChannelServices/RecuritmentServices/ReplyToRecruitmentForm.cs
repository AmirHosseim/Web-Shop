using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.RecuritmentServices
{
    public class ReplyToRecruitmentForm
    { 
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int RecruitmenFormtId { get; set; }

        public bool WasNotAccept { get; set; }

        [ForeignKey("RecruitmenFormtId")]
        public RecruitmentInChannelForm recruitmentInChannelForm { get; set; }

        public IdentityUser User { get; set; }
    }
}
