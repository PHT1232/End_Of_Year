using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.SumaryService.dtos
{
    public class ProductTopSalesList
    {
        public List<ProductTopSales> items { get; set; }
    }

    public class ProductTopSales
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }
        public float ProductSales { get; set; }
    }
}
