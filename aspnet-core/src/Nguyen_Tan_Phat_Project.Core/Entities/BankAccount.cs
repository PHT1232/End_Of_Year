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
    public class BankAccount : Entity
    {
        [Key]
        public string BankId { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankCity { get; set; }
    }
}
