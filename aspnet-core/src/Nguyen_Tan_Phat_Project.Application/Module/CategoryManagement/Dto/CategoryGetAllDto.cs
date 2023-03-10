using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.CategoryManagement.Dto
{
    public class CategoryGetAllDto
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string Username { get; set; }
    }
}
