using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class EditChannelViewModel
    {
        public string Name { get; set; }

        public string BioGraphy { get; set; }

        public string CreatorId { get; set; }

        public string ChannelId { get; set; }

        public bool AlreadySpecialUserAccount { get; set; }
    }
}
