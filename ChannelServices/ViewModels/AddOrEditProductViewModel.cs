using ChannelServices.CategoriesServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class AddOrEditProductViewModel
    {
        public string ProductIdForEdit { get; set; }

        [Required(ErrorMessage = "لطفا نام محصول را وارد کنید")]
        [Display(Name = "نام محصول")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لطفا توضیحات محصول را وارد کنید")]
        [Display(Name = "توضیحات محصول")]
        public string Description { get; set; }

        [Required(ErrorMessage = "لطفا قیمت محصول را وارد کنید")]
        [Display(Name = "قیمت محصول")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "لطفا دسته بندی محصول را وارد کنید")]
        public int CategoryId { get; set; }

        public bool AllowToAddOrEditProduct { get; set; }

        public string ChannelId { get; set; }

        public List<Category> Categories { get; set; }
    }
}
