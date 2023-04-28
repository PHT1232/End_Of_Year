﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
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
        public CMND EmployeeCMND { get; set; }
        public string JobTitle { get; set; }
        public Structure WorkUnit { get; set; }
        public string TaxIdentification { get; set; }
        public int EmployeeSalary { get; set; }
        public float SalaryFactor { get; set; }
        public string TypeOfContract { get; set; }
        public BankAccount EmployeeBankAccount { get; set; }
    }
}
    