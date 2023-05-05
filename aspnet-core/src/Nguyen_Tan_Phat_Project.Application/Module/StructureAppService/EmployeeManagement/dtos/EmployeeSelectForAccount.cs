using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement.dtos
{
    public class EmployeeSelectForAccountList
    {
        public List<EmployeeSelectForAccount> items {  get; set; }
    }
    public class EmployeeSelectForAccount
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
