using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class AddRecuritmentFormViewModel
    {
        [Required(ErrorMessage ="لطفا توضیحات فرم را وارد کنید")]
        [MinLength(8,ErrorMessage ="توضیحات نمی تواند کمتر از 8 کاراکتر باشد")]
        [MaxLength(100, ErrorMessage = "توضیحات نمی تواند کمتر از 100 کاراکتر باشد")]
        public string Description { get; set; }

        public string ChannelId { get; set; }
    }
}
