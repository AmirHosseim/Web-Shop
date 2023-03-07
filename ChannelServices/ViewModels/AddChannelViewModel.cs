using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class AddChannelViewModel
    {
        [MaxLength(20,ErrorMessage ="نام کانال نمی تواند بیشتر از 20 کاراکتر باشد")]
        [Required(ErrorMessage ="این فیلد اجباری است")]
        public string Name { get; set; }

        [MaxLength(20, ErrorMessage = "توضیحات کانال نمی تواند بیشتر از 40 کاراکتر باشد")]
        [Required(ErrorMessage = "این فیلد اجباری است")]
        public string BioGraphy { get; set; }

        public string CreatorId { get; set; }

        public bool AllowToCreate { get; set; }
    }
}
