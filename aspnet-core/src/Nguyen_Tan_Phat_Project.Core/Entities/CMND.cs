using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class CMND : Entity
    {
        [Key]
        public string SoCMND { get; set; }
        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string QuocTich { get; set; }
    }
}
