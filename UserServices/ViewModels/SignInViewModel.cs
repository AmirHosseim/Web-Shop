using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace UserServices.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "لطفا نام کاربری را وارد")]
        [Display(Name = "نام کاربری")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفا رمز عبور را وارد")]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RemmberMe { get; set; }

        public string returnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
