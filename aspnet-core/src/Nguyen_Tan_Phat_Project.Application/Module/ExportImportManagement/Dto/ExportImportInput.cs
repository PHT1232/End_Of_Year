using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ExportImportManagement.Dto
{
    public class ExportImportInput
    {
        public string ExportImportCode { get; set; }
        public long OrderCreator { get; set; }
        public int OrderType { get; set; }
        public int OrderStatus { get; set; }
        public CustomerDto Customer { get; set; }
        public List<ExportImportProductDto> Products { get; set; }
        public string StorageId { get; set; }
        public string StorageInputId { get; set; }
        public float TotalPrice { get; set; }
        public string Description { get; set; }
    }
}
