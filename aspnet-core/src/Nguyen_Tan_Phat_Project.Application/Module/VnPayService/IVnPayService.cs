using Microsoft.AspNetCore.Http;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(string id);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
