using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.StructureManagement.dtos
{
    public class StructureOutputDto
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Address { get; set; }
        public string LevelOfUnit { get; set; }
        public string UnitOf { get; set; }
        public int BusinessRN { get; set; }
        public DateTime IssuedDate { get; set; }
        public string IssuedPlace { get; set; }
    }
}
