using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto
{
    public class StorageProductDetailList
    {
        public List<StorageProductDetail> items { get; set; }
    }
    public class StorageProductDetail
    {
        public int StorageProductId { get; set; }
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public int Quantity { get; set; }
        public string ProductLocation { get; set; }
    }
}
