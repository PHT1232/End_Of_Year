using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ProductManagement.Dto
{
    public class ProductStorageDto
    {
        public string StorageCode { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductLocation { get; set; }
        public string Description { get; set; }
    }
}
