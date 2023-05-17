using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.IncomeExpensesAppservice.incomeExpensesManagement.Dtos
{
    public class ExpensesInputDto
    {
        public string ExpensesCode { get; set; }
        public string EmployeeCode { get; set; }
        public int OrderStatus { get; set; }
        public string ProductProvider { get; set; }
        public List<ExpensesProductDto> Products { get; set; }
        public string StorageId { get; set; }
        public float Discount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }
        public float TotalPrice { get; set; }
    }
}
