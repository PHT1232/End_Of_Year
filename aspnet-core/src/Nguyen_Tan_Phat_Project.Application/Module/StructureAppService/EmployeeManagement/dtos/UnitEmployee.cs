using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement.dtos
{
    public class UnitEmployeeList
    {
        public List<UnitEmployee> items { get; set; }
    }
    public class UnitEmployee
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
    }
}
