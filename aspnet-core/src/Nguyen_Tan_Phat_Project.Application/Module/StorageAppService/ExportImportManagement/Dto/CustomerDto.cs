using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto
{

    public class CustomerDto
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAdress { get; set; }
        public float Discount { get; set; }
    }
}
