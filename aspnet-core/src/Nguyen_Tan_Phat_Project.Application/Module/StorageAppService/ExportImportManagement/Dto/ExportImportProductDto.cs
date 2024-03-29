﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto
{
    public class ExportImportProductDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public byte[] PictureImage { get; set; }
        public string StorageId { get; set; }
        public string StorageInputId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Unit { get; set; }
        public string Location { get; set; }
        public float FinalPrice { get; set; }
    }
}
