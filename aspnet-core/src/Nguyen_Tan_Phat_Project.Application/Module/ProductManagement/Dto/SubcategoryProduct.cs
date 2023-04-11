using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto
{
    public class SubcategoryProductList
    {
        public List<SubcategoryProduct> items { get; set; }
    }
    public class SubcategoryProduct
    {
        public int SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
    }
}
