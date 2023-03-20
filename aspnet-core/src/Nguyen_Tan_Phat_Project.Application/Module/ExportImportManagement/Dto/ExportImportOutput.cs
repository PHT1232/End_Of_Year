﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ExportImportManagement.Dto
{
    public class ExportImportOutput
    {
        public string ExportImportCode { get; set; }
        public string NameOfReceiver { get; set; }
        public int OrderStatus { get; set; }
        public string ReceiveAddress { get; set; }
        public List<ExportImportProductDto> Products { get; set; }
        public string StorageId { get; set; }
        public float TotalPrice { get; set; }
    }
}
