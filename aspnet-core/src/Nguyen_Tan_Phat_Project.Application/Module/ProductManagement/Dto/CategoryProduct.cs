using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto
{
    public class CategoryProductList
    {
        public List<CategoryProduct> items { get; set; }
    }
    public class CategoryProduct
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
