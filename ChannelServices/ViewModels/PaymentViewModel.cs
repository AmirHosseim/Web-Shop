using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class PaymentViewModel
    {
        [Required]
        public long OrderId { get; set; }

        [Required]
        public decimal Value { get; set; }

        [Required(ErrorMessage ="لطفا شماره کارت را وارد کنید")]
        [MinLength(16,ErrorMessage ="شماره کرات باید 16 رقمی باشد")]
        public string cardnumber { get; set; }

        [Required(ErrorMessage = "لطفا رمز کارت را وارد کنید")]
        public string CardPassword { get; set; }

        [Required(ErrorMessage = "لطفا CVV2 کارت را وارد کنید")]
        public long CVV2 { get; set; }
    }
}
