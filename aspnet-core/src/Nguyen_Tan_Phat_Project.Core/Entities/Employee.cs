using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class Employee : FullAuditedEntity<string>
    {
        public string EmployeeName { get; set; }
        public string EmployeeGender { get; set; }
        public DateTime EmployeeDateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public int phoneNumber { get; set; }
        [ForeignKey("Structure")]
        public string WorkUnitId { get; set; }
        public Structure WorkUnit { get; set; }
        public string TaxIdentification { get; set; }
        public int EmployeeSalary { get; set; }
        public int EmployeeAllowance { get; set; }
        public float SalaryFactor { get; set; }
        public string TypeOfContract { get; set; }
        [ForeignKey("BankAccount")]
        public string BankId { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
    