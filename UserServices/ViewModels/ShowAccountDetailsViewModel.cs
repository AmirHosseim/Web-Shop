using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServices.UserFavorite;

namespace UserServices.ViewModels
{
    public class ShowAccountDetailsViewModel
    {
        public IdentityUser User { get; set; }

        public List<Favorite> Favorites { get; set; }
    }
}
