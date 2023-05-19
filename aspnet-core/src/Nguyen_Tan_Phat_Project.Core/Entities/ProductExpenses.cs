using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Entities
{
    public class ProductExpenses : FullAuditedEntity
    {
        [ForeignKey("Expenses")]
        public string ExpensesId { get; set; }
        public Expenses Expenses { get; set; }
        [ForeignKey("Product")]
        public string ProductCode { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Location { get; set; }
        public float FinalPrice { get; set; }
    }
}
