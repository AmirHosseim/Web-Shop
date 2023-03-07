using ChannelServices.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelServices.ViewModels
{
    public class ChannelIncomeViewModel
    {
        public decimal ChannelIncome { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        public List<Order> Orders { get; set; }

        public List<SettelmentReceipt> SettelmentReceipts { get; set; }

        public List<SellsProduct> SellsProducts { get; set; }

    }

    public class SellsProduct
    {
        public Product product { get; set; }

        public int SellQuantity { get; set; }
        public decimal SellValue { get; set; }

        public List<Order> order { get; set; }
    }

}
