using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.UserManager;

namespace UserServices.ViewModels
{
    public class CreatSpecialUserViewModel
    {
        public int OutTimeDay { get; set; }

        public string UserId { get; set; }

        public List<OutTimeDay> OutTimeDays { get; set; }
    }
}
