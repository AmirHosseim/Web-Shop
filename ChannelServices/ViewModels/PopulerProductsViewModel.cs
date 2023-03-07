using ChannelServices.ProductServices;

namespace ChannelServices.ViewModels
{
    public class PopulerProductsViewModel
    {
        public string ProductId { get; set; }

        public int SillQuantity { get; set; }

        public Product product { get; set; }
    }
}
