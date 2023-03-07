using ChannelServices.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.Repositories
{
    public interface IGetProducts
    {
        public ShowProductsViewModel ShowProductsViewModel(int PageId, List<Product> products);
    }

    public class GetProducts : IGetProducts
    {
        public ShowProductsViewModel ShowProductsViewModel(int PageId, List<Product> products)
        {
            int take = 30;

            int skip = (PageId - 1) * take;

            ShowProductsViewModel list = new();

            list.CurrentPage = PageId;
            list.PageCount = (int)Math.Ceiling(products.Count() / (double)take);

            list.Products = products.Skip(skip).Take(take).ToList();

            return list;
        }
    }

    public class ShowProductsViewModel
    {
        public List<Product> Products { get; set; }

        public int PageCount { get; set; }

        public int CurrentPage { get; set; }

        public string SearchString { get; set; }
    }
}
