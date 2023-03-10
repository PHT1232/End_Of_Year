using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageManagement.Dto
{
    public class StorageGetAllDto
    {
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public string Address { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastDateModified { get; set; }
        public string Username { get; set; }
    }
}
