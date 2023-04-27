using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class CMND
    {
        [Key]
        public string SoCMND { get; set; }
        public string NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string QuocTich { get; set; }
    }
}
