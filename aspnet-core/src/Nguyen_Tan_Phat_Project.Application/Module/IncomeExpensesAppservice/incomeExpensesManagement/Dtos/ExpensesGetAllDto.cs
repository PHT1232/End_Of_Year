using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Dtos
{
    public class ExpensesGetAllDto
    {
        public string ExpensesCode { get; set; }
        public string ProductProvider { get; set; }
        public string StorageName { get; set; }
        public string Description { get; set; }
        public DateTime PaymentDate { get; set; }
        public int OrderStatus { get; set; }
        public float TotalPrice { get; set; }
        public float TotalPriceAfterDiscount { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
