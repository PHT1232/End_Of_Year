using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto
{
    public class ExportImportGetAllDto
    {
        public string ExportImportCode { get; set; }
        public string NameOfReceiver { get; set; }
        public string StorageName { get; set; }
        public int OrderStatus { get; set; }
        public int OrderType { get; set; }
        public float TotalPrice { get; set; }
        public DateTime CreationTime { get; set; }
        public string NameOfExport { get; set; }
        public string Username { get; set; }
    }
}
