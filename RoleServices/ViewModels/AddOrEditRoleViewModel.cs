using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleServices.ViewModels
{
    public class AddOrEditRoleViewModel
    {
        [Required(ErrorMessage = "لطفا نام مقام را وارد کنید")]
        [Display(Name = "نام مقام")]
        public string Name { get; set; }

        public string RoleId { get; set; }
    }
}
