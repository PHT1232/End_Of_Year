using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto
{
    public class ListOfCustomer
    {
        public List<CustomerListDto> items {  get; set; }
    }

    public class CustomerListDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
