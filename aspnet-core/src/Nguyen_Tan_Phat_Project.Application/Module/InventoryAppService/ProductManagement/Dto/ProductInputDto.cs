using Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.InventoryManagement.ProductManagement.Dto
{
    public class ProductInputDto
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDetail { get; set; }
        public float Price { get; set; }
        public string CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public string Unit { get; set; }
        public List<StorageProductDetail> storages { get; set; }
    }
}
