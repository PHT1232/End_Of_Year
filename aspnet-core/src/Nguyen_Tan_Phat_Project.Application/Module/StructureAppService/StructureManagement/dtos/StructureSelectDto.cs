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
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
