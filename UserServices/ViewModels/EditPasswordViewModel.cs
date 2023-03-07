using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.ViewModels
{
    public class EditPasswordViewModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string UserId { get; set; }
    }
}
