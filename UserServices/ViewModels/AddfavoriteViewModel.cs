using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.ViewModels
{
    public class AddfavoriteViewModel
    {
        public AddfavoriteViewModel()
        {
            CategoriesVm = new List<CategoriesViewModel>();
        }

        public string UserId { get; set; }
        public List<CategoriesViewModel> CategoriesVm { get; set; }
    }

    public class CategoriesViewModel
    {
        public string CategoryName { get; set; }

        public int CategoryId { get; set; }

        public bool IsSelected { get; set; }
    }
}
