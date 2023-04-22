using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto
{
    public class LookUpTableList
    {
        public List<LookUpTable> items { get; set; }
    }

    public class LookUpTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
