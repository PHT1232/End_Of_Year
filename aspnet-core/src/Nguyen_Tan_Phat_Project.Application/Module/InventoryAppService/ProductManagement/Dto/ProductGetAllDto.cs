using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto
{
    public class ListProductGetAll
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string? CategoryName { get; set; }
        public string ProductImage { get; set; }
        public float Price { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public List<ProductGetAllDto> Products { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastDateModified { get; set; }
        public string? Username { get; set; }
    }
    public class ProductGetAllDto
    {
        public string StorageCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string? CategoryName { get; set; }
        //public string ProductImage { get; set; }
        public float Price { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public string InventoryStatus { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastDateModified { get; set; }
        public string? Username { get; set; }
    }
}
