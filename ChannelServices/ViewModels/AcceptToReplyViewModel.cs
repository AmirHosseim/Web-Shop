using ChannelServices.RecuritmentServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class AcceptToReplyViewModel
    {
        //EmployId
        public string UserId { get; set; }

        public int formId { get; set; }

        public string ChannelId { get; set; }

        public List<AccessSelection> AccessSelections { get; set; }
    }
    public class AccessSelection
    {
        public Access access { get; set; }

        public bool IsSelect { get; set; }
    }
}
