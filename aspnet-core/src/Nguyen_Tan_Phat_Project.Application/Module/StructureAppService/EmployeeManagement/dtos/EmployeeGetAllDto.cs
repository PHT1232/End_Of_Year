using Nguyen_Tan_Phat_Project.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement.dtos
{
    public class EmployeeGetAllDto
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string JobTitle { get; set; }
        public string WorkUnit { get; set; }
        public int EmployeePhone { get; set; }
        public int EmployeeSalary { get; set; }
        public int EmployeeAllowance { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
    }
}
