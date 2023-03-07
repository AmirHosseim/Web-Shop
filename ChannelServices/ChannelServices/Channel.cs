using Microsoft.AspNetCore.Identity;
using ChannelServices.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChannelServices.Following_Services;
using ChannelServices.RecuritmentServices;

namespace ChannelServices.ChannelServices;

public class Channel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string BioGraphy { get; set; }

    public string CreatorId { get; set; }

    public IdentityUser User { get; set; }

    public List<Product> products { get; set; }

    public List<Follow> followers { get; set; }

    public List<RecruitmentInChannelForm> recruitmentInChannelForms { get; set; }

    public List<RecruitmentInChannel> RecruitmentsInChannel { get; set; }
}
