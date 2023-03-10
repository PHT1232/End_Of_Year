using Nguyen_Tan_Phat_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.CategoryManagement.Dto
{
    public class CategoryInputDto
    {
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public List<string> SubCategorys { get; set; }
    }
}
