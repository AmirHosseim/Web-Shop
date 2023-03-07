using ChannelServices.ChannelServices;
using ChannelServices.RecuritmentServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class ChannelManagmentViewModel
    {
        public List<AccessToEmploy> AccessToEmploys { get; set; }

        public Channel Channel { get; set; }

        public bool AllowToManageChannel { get; set; }
    }
}
