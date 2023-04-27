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
        public Structure WorkUnit { get; set; }
        public string TaxIdentification { get; set; }
        public BankAccount EmployeeBankAccount { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastDateModified { get; set; }
    }
}
