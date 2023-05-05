using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.StructureManagement.dtos
{
    public class StructureSelectListDto
    {
        public List<StructureSelectDto> items { get; set; }
    }

    public class StructureSelectDto
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
    }
}
