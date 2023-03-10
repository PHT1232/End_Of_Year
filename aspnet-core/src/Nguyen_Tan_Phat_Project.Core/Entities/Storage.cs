using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class Storage : FullAuditedEntity<string>
    {
        public string StorageName { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
    }
}
