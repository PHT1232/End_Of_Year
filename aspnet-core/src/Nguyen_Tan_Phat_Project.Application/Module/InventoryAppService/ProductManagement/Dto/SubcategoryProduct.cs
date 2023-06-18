using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.InventoryManagement.ProductManagement.Dto
{
    public class SubcategoryProductList
    {
        public List<SubcategoryProduct> items { get; set; }
    }
    public class SubcategoryProduct
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }
}
