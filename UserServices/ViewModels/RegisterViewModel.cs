using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفا نام کاربری را وارد")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "لطفا رمز عبور را وارد")]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "با رمز عبور یکی نیست")]
        [Required(ErrorMessage = "لطفا رمز عبور را تکرار کنید")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "لطفا ایمیل را وارد")]
        [Display(Name = "ایمیل")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool QuickLogIn { get; set; }
    }
}
