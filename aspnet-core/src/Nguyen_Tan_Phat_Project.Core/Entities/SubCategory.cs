using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class SubCategory : FullAuditedEntity
    {
        [ForeignKey("Category")]
        public string CategoryId { get; set; }
        public Category Category { get; set; }
        public string SubCategoryName { get; set; }
    }
}
