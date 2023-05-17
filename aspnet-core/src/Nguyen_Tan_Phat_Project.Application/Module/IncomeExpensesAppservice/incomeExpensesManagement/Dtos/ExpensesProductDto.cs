using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Dtos
{
    public class ExpensesProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Unit { get; set; }
        public string Location { get; set; }
        public float FinalPrice { get; set; }
    }
}
