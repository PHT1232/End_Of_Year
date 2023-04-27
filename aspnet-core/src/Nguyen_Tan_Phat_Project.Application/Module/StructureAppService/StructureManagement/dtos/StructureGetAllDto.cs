using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.StructureManagement.dtos
{
    public class StructureGetAllDto
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Address { get; set; }
        public string LevelOfUnit { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastDateModified { get; set; }
        public string username { get; set; }
    }
}
