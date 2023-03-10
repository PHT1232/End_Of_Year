using Nguyen_Tan_Phat_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageManagement.Dto
{
    public class StorageForUpdate
    {
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }
}
