using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto
{
    public class ExportImportOutput
    {
        public string ExportImportCode { get; set; }
        public long OrderCreator { get; set; }
        public int OrderStatus { get; set; }
        public int OrderType { get; set; }
        public string ReceiveAddress { get; set; }
        public CustomerDto Customer { get; set; }
        public List<ExportImportProductDto> Products { get; set; }
        public string StorageId { get; set; }
        public string StorageInputId { get; set; }
        public string NameOfExport { get; set; }
        public float TotalPrice { get; set; }
    }
}
