using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class Test : FullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public int TestVarible { get; set; }
    }
}
