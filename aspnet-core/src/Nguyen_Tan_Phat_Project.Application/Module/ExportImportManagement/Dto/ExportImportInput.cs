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
        public string OrderCreator { get; set; }
        public CustomerDto Customer { get; set; }
        public List<ExportImportProductDto> Products { get; set; }
        public string StorageId { get; set; }
        public float TotalPrice { get; set; }
        public string Description { get; set; }
    }
}
