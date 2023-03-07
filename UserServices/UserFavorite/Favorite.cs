using ChannelServices.CategoriesServices;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.UserFavorite
{
    public class Favorite
    {
        public long Id { get; set; }

        public int CategoriId { get; set; }

        public string UserId { get; set; }

        [ForeignKey("CategoriId")]
        public Category Category { get; set; }

        public IdentityUser User { get; set; }
    }
}
