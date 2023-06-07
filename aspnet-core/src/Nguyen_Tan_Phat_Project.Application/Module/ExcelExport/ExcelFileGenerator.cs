using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using Nguyen_Tan_Phat_Project.Module.StructureAppService.EmployeeManagement.dtos;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.ExcelExport
{
    public class ConvertClass
    {
        public DataTable ConvertToDataTable(List<ExportImportProductDto> list)
        {
            DataTable dataTable = new DataTable(typeof(ExportImportProductDto).Name);

            PropertyInfo[] propertyInfos = typeof(ExportImportProductDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                dataTable.Columns.Add(propertyInfo.Name);
            }

            foreach (var item in list)
            {
                var values = new Object[propertyInfos.Length];
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    values[i] = propertyInfos[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }

    public class QRCodeGen
    {
        public Byte[] GenerateQRCode(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode bm = new BitmapByteQRCode(qrCodeData);
            Byte[] qrCodeImage = bm.GetGraphic(20);
            return qrCodeImage;
            //qrCodeImage.Save("qrcode.png", ImageFormat.Png);
        }
    }

    public class ExcelFileGenerator
    {
        public void SetFormatCell(ref IXLWorksheet worksheet, int row, int cell, bool isItalic, bool isBold, XLAlignmentHorizontalValues valuesHorizontal, XLAlignmentVerticalValues verticalValues)
        {
            worksheet.Cell(row, cell).Style.Alignment.SetHorizontal(valuesHorizontal);
            worksheet.Cell(row, cell).Style.Alignment.SetVertical(verticalValues);
            worksheet.Cell(row, cell).Style.Font.FontSize = 12;
            worksheet.Cell(row, cell).Style.Font.SetItalic(isItalic);
            worksheet.Cell(row, cell).Style.Font.SetBold(isBold);
            worksheet.Cell(row, cell).Style.Font.FontName = "Times New Roman";
        }

        public void SetBorderCell(ref IXLWorksheet worksheet, int row, int startCell, int endCell)
        {

            for (int i = startCell; i <= endCell; i++)
            {
                worksheet.Cell(row, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
        }

        public byte[] GenerateExcelFileForExportImport(List<ExportImportProductDto> list, ExportImport exportImport, Customer customer, Employee employee)
        {
            XLWorkbook workbook = new XLWorkbook("E:\\Documents\\GitHub\\End_Of_Year\\aspnet-core\\src\\Nguyen_Tan_Phat_Project.Web.Host\\wwwroot\\ExcelTemplate\\phieu xuat kho sua.xlsx");
            IXLWorksheet worksheet = workbook.Worksheet("Sheet1");

            int rowNumber = 13, STT = 0;

            worksheet.Cell(6, 4).Value = "Ngày " + exportImport.CreationTime.Day + " tháng " + exportImport.CreationTime.Month + " năm " + exportImport.CreationTime.Year;
            worksheet.Cell(6, 4).Style.Font.FontSize = 12;
            worksheet.Cell(6, 4).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(6, 4).Style.Font.SetBold(true);

            worksheet.Cell(7, 5).Value = "Số: " + exportImport.Id.ToString();
            worksheet.Cell(7, 5).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(7, 5).Style.Font.FontSize = 12;
            worksheet.Cell(7, 5).Style.Font.SetBold(true);

            worksheet.Cell(9, 4).Value = customer.CustomerName;
            worksheet.Cell(9, 4).Style.Font.FontSize = 12;
            worksheet.Cell(9, 4).Style.Font.FontName = "Times New Roman";

            worksheet.Cell(10, 4).Value = customer.CustomerAddress;
            worksheet.Cell(10, 4).Style.Font.FontSize = 12;
            worksheet.Cell(10, 4).Style.Font.FontName = "Times New Roman";

            worksheet.Cell(11, 4).Value = exportImport.StructureId;
            worksheet.Cell(11, 4).Style.Font.FontSize = 12;
            worksheet.Cell(11, 4).Style.Font.FontName = "Times New Roman";

            foreach (var product in list)
            {
                rowNumber++;
                STT++;
                IXLRange range = worksheet.Range("C" + rowNumber + ":" + "E" + rowNumber);
                IXLRange range2 = worksheet.Range("F" + rowNumber + ":" + "I" + rowNumber);
                range.Merge();
                range2.Merge();
                worksheet.Cell(rowNumber, 1).Value = STT.ToString();
                worksheet.Cell(rowNumber, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 1).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 1).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 2).Value = product.StorageId;
                worksheet.Cell(rowNumber, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 2).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 2).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 3).Value = product.ProductId;
                worksheet.Cell(rowNumber, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 3).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 3).Style.Font.FontName = "Times New Roman";
                worksheet.Cell(rowNumber, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(rowNumber, 6).Value = product.ProductName;
                worksheet.Cell(rowNumber, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 6).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 6).Style.Font.FontName = "Times New Roman";
                worksheet.Cell(rowNumber, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                
                worksheet.Cell(rowNumber, 10).Value = product.Location;
                worksheet.Cell(rowNumber, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 10).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 10).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 11).Value = product.Unit;
                worksheet.Cell(rowNumber, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 11).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 11).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 12).Value = product.Quantity.ToString();
                worksheet.Cell(rowNumber, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 12).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 12).Style.Font.FontName = "Times New Roman";
            }

            IXLRange range3 = worksheet.Range("J" + (rowNumber + 3) + ":" + "L" + (rowNumber + 3));
            range3.Merge();
            worksheet.Cell(rowNumber + 3, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(rowNumber + 3, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Cell(rowNumber + 3, 10).Value = "Ngày ..... tháng ..... năm .........";
            worksheet.Cell(rowNumber + 3, 10).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 3, 10).Style.Font.SetItalic(true);
            worksheet.Cell(rowNumber + 3, 10).Style.Font.FontName = "Times New Roman";

            IXLRange range4 = worksheet.Range("B" + (rowNumber + 4) + ":" + "D" + (rowNumber + 4));
            range4.Merge();
            worksheet.Cell(rowNumber + 4, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(rowNumber + 4, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Cell(rowNumber + 4, 2).Value = "Người nhận hàng";
            worksheet.Cell(rowNumber + 4, 2).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 4, 2).Style.Font.SetBold(true);
            worksheet.Cell(rowNumber + 4, 2).Style.Font.FontName = "Times New Roman";

            IXLRange range5 = worksheet.Range("F" + (rowNumber + 4) + ":" + "H" + (rowNumber + 4));
            range5.Merge();
            worksheet.Cell(rowNumber + 4, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(rowNumber + 4, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Cell(rowNumber + 4, 6).Value = "Người lập phiếu";
            worksheet.Cell(rowNumber + 4, 6).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 4, 6).Style.Font.SetBold(true);
            worksheet.Cell(rowNumber + 4, 6).Style.Font.FontName = "Times New Roman";

            IXLRange range6 = worksheet.Range("J" + (rowNumber + 4) + ":" + "L" + (rowNumber + 4));
            range6.Merge();
            worksheet.Cell(rowNumber + 4, 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(rowNumber + 4, 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Cell(rowNumber + 4, 10).Value = "Thủ kho";
            worksheet.Cell(rowNumber + 4, 10).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 4, 10).Style.Font.SetBold(true);
            worksheet.Cell(rowNumber + 4, 10).Style.Font.FontName = "Times New Roman";

            IXLRange range7 = worksheet.Range("B" + (rowNumber + 10) + ":" + "D" + (rowNumber + 10));
            range7.Merge();
            worksheet.Cell(rowNumber + 10, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(rowNumber + 10, 2).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Cell(rowNumber + 10, 2).Value = customer.CustomerName;
            worksheet.Cell(rowNumber + 10, 2).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 10, 2).Style.Font.SetItalic(true);
            worksheet.Cell(rowNumber + 10, 2).Style.Font.FontName = "Times New Roman";

            IXLRange range8 = worksheet.Range("F" + (rowNumber + 10) + ":" + "H" + (rowNumber + 10));
            range8.Merge();
            worksheet.Cell(rowNumber + 10, 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(rowNumber + 10, 6).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            worksheet.Cell(rowNumber + 10, 6).Value = employee.EmployeeName;
            worksheet.Cell(rowNumber + 10, 6).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 10, 6).Style.Font.SetItalic(true);
            worksheet.Cell(rowNumber + 10, 6).Style.Font.FontName = "Times New Roman";

            if (exportImport.OrderStatus != 2)
            {
                QRCodeGen qr = new QRCodeGen();
                var byteImage = qr.GenerateQRCode("https://unten.tech:44311/api/services/app/ExportImport/UpdateOrderQR?exportImportCode=" + exportImport.Id.ToString() + "&orderStatus=2");
                var mStream = new MemoryStream(byteImage);

                var picture = worksheet.AddPicture(mStream);

                picture.MoveTo(worksheet.Cell("E" + (rowNumber + 15))).Scale(0.2);
            }

            MemoryStream memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        public byte[] GenerateDeliveryExcel(List<ExportImportProductDto> list, ExportImport exportImport, Customer customer, Employee employee, Employee deliveryEmployee)
        {
            XLWorkbook workbook = new XLWorkbook("E:\\Documents\\GitHub\\End_Of_Year\\aspnet-core\\src\\Nguyen_Tan_Phat_Project.Web.Host\\wwwroot\\ExcelTemplate\\phieu xuat hang.xlsx");
            IXLWorksheet worksheet = workbook.Worksheet("Sheet1");

            int rowNumber = 13, STT = 0;
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");

            worksheet.Cell(6, 4).Value = "Ngày " + exportImport.CreationTime.Day + " tháng " + exportImport.CreationTime.Month + " năm " + exportImport.CreationTime.Year;
            worksheet.Cell(6, 4).Style.Font.FontSize = 12;
            worksheet.Cell(6, 4).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(6, 4).Style.Font.SetBold(true);

            worksheet.Cell(8, 10).Value = "Số: " + exportImport.Id.ToString();
            worksheet.Cell(8, 10).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(8, 10).Style.Font.FontSize = 12;
            worksheet.Cell(8, 10).Style.Font.SetBold(true);

            worksheet.Cell(8, 4).Value = customer.Id;
            worksheet.Cell(8, 4).Style.Font.FontSize = 12;
            worksheet.Cell(8, 4).Style.Font.FontName = "Times New Roman";

            worksheet.Cell(9, 4).Value = customer.CustomerName;
            worksheet.Cell(9, 4).Style.Font.FontSize = 12;
            worksheet.Cell(9, 4).Style.Font.FontName = "Times New Roman";

            worksheet.Cell(10, 4).Value = customer.CustomerAddress;
            worksheet.Cell(10, 4).Style.Font.FontSize = 12;
            worksheet.Cell(10, 4).Style.Font.FontName = "Times New Roman";

            worksheet.Cell(11, 4).Value = exportImport.Description;
            worksheet.Cell(11, 4).Style.Font.FontSize = 12;
            worksheet.Cell(11, 4).Style.Font.FontName = "Times New Roman";

            foreach (var product in list)
            {
                rowNumber++;
                STT++;
                IXLRange range = worksheet.Range("B" + rowNumber + ":" + "D" + rowNumber);
                IXLRange range2 = worksheet.Range("E" + rowNumber + ":" + "H" + rowNumber);
                range.Merge();
                range2.Merge();
                worksheet.Cell(rowNumber, 1).Value = STT.ToString();
                worksheet.Cell(rowNumber, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 1).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 1).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 2).Value = product.ProductId;
                worksheet.Cell(rowNumber, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 2).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 2).Style.Font.FontName = "Times New Roman";
                worksheet.Cell(rowNumber, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(rowNumber, 5).Value = product.ProductName;
                worksheet.Cell(rowNumber, 5).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 5).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 5).Style.Font.FontName = "Times New Roman";
                worksheet.Cell(rowNumber, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(rowNumber, 9).Value = product.Unit;
                worksheet.Cell(rowNumber, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 9).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 9).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 10).Value = product.Quantity.ToString();
                worksheet.Cell(rowNumber, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 10).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 10).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 11).Value = double.Parse(product.Price.ToString()).ToString("#,###", cul.NumberFormat);
                worksheet.Cell(rowNumber, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 11).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 11).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 12).Value = double.Parse(product.FinalPrice.ToString()).ToString("#,###", cul.NumberFormat); 
                worksheet.Cell(rowNumber, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 12).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 12).Style.Font.FontName = "Times New Roman";
            }
            
            
            IXLRange rangeCong = worksheet.Range("A" + (rowNumber + 1) + ":" + "D" + (rowNumber + 1));
            rangeCong.Merge();
            worksheet.Cell(rowNumber + 1, 1).Value = "Cộng";
            SetBorderCell(ref worksheet, rowNumber + 1, 1, 4);
            SetFormatCell(ref worksheet, rowNumber + 1, 1, false, true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center);

            IXLRange rangeCongTien = worksheet.Range("E" + (rowNumber + 1) + ":" + "L" + (rowNumber + 1));
            rangeCongTien.Merge();
            worksheet.Cell(rowNumber + 1, 5).Value = double.Parse(exportImport.TotalPrice.ToString()).ToString("#,###", cul.NumberFormat);
            SetBorderCell(ref worksheet, rowNumber + 1, 5, 12);
            SetFormatCell(ref worksheet, rowNumber + 1, 5, false, true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center);

            IXLRange tyLeCK = worksheet.Range("A" + (rowNumber + 2) + ":" + "F" + (rowNumber + 2));
            tyLeCK.Merge();
            worksheet.Cell(rowNumber + 2, 1).Value = "Tỷ lệ CK: " + exportImport.Discount + "%";
            SetBorderCell(ref worksheet, rowNumber + 2, 1, 6);
            worksheet.Cell(rowNumber + 2, 1).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 2, 1).Style.Font.FontName = "Times New Roman";

            IXLRange soTienCK = worksheet.Range("G" + (rowNumber + 2) + ":" + "I" + (rowNumber + 2));
            soTienCK.Merge();
            worksheet.Cell(rowNumber + 2, 7).Value = "Số tiền chiết khấu: ";
            SetBorderCell(ref worksheet, rowNumber + 2, 7, 12);
            worksheet.Cell(rowNumber + 2, 7).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 2, 7).Style.Font.FontName = "Times New Roman";

            IXLRange soTienCKNumber = worksheet.Range("J" + (rowNumber + 2) + ":" + "L" + (rowNumber + 2));
            soTienCKNumber.Merge();
            worksheet.Cell(rowNumber + 2, 10).Value = double.Parse((exportImport.TotalPrice * (exportImport.Discount / 100)).ToString()).ToString("#,###", cul.NumberFormat);
            SetFormatCell(ref worksheet, rowNumber + 2, 10, false, true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center);

            IXLRange mergeCell1 = worksheet.Range("A" + (rowNumber + 3) + ":" + "F" + (rowNumber + 3));
            mergeCell1.Merge();
            SetBorderCell(ref worksheet, rowNumber + 3, 1, 6);

            IXLRange congTienHang = worksheet.Range("G" + (rowNumber + 3) + ":" + "I" + (rowNumber + 3));
            congTienHang.Merge();
            worksheet.Cell(rowNumber + 3, 7).Value = "Cộng tiền hàng (Đã trừ CK): ";
            SetBorderCell(ref worksheet, rowNumber + 3, 7, 12);
            worksheet.Cell(rowNumber + 3, 7).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 3, 7).Style.Font.FontName = "Times New Roman";

            IXLRange congTienHangNumber = worksheet.Range("J" + (rowNumber + 3) + ":" + "L" + (rowNumber + 3));
            congTienHangNumber.Merge();
            worksheet.Cell(rowNumber + 3, 10).Value = double.Parse((exportImport.TotalPrice - (exportImport.TotalPrice * (exportImport.Discount / 100))).ToString()).ToString("#,###", cul.NumberFormat);
            SetFormatCell(ref worksheet, rowNumber + 3, 10, false, true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center);

            IXLRange mergeCell2 = worksheet.Range("A" + (rowNumber + 4) + ":" + "F" + (rowNumber + 4));
            mergeCell2.Merge();
            SetBorderCell(ref worksheet, rowNumber + 4, 1, 6);

            IXLRange tongTien = worksheet.Range("G" + (rowNumber + 4) + ":" + "I" + (rowNumber + 4));
            tongTien.Merge();
            worksheet.Cell(rowNumber + 4, 7).Value = "Tổng tiền thanh toán: ";
            SetBorderCell(ref worksheet, rowNumber + 4, 7, 12);
            worksheet.Cell(rowNumber + 4, 7).Style.Font.SetBold(true);
            worksheet.Cell(rowNumber + 4, 7).Style.Font.FontSize = 12;
            worksheet.Cell(rowNumber + 4, 7).Style.Font.FontName = "Times New Roman";

            IXLRange tongTienNumber = worksheet.Range("J" + (rowNumber + 4) + ":" + "L" + (rowNumber + 4));
            tongTienNumber.Merge();
            worksheet.Cell(rowNumber + 4, 10).Value = double.Parse((exportImport.TotalPrice - (exportImport.TotalPrice * (exportImport.Discount / 100))).ToString()).ToString("#,###", cul.NumberFormat);
            SetFormatCell(ref worksheet, rowNumber + 4, 10, false, true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center);

            IXLRange range3 = worksheet.Range("J" + (rowNumber + 3) + ":" + "L" + (rowNumber + 3));
            range3.Merge();
            worksheet.Cell(rowNumber + 7, 11).Value = "Ngày ..... tháng ..... năm .........";
            SetFormatCell(ref worksheet, rowNumber + 7, 11, true, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center);

            IXLRange range4 = worksheet.Range("B" + (rowNumber + 8) + ":" + "D" + (rowNumber + 8));
            range4.Merge();
            worksheet.Cell(rowNumber + 8, 2).Value = "Người nhận hàng";
            SetFormatCell(ref worksheet, rowNumber + 8, 2, false, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center);

            IXLRange range5 = worksheet.Range("F" + (rowNumber + 8) + ":" + "H" + (rowNumber + 8));
            range5.Merge();
            worksheet.Cell(rowNumber + 8, 6).Value = "Người lập phiếu";
            SetFormatCell(ref worksheet, rowNumber + 8, 6, false, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center);

            IXLRange range6 = worksheet.Range("J" + (rowNumber + 8) + ":" + "L" + (rowNumber + 8));
            range6.Merge();
            worksheet.Cell(rowNumber + 8, 10).Value = "Nhân viên giao hàng";
            SetFormatCell(ref worksheet, rowNumber + 8, 10, false, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center);

            IXLRange range7 = worksheet.Range("B" + (rowNumber + 13) + ":" + "D" + (rowNumber + 13));
            range7.Merge();
            worksheet.Cell(rowNumber + 13, 2).Value = customer.CustomerName;
            SetFormatCell(ref worksheet, rowNumber + 13, 2, true, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center);

            IXLRange range8 = worksheet.Range("F" + (rowNumber + 13) + ":" + "H" + (rowNumber + 13));
            range8.Merge();
            worksheet.Cell(rowNumber + 13, 6).Value = employee.EmployeeName;
            SetFormatCell(ref worksheet, rowNumber + 13, 6, true, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center);

            IXLRange range9 = worksheet.Range("J" + (rowNumber + 13) + ":" + "L" + (rowNumber + 13));
            range9.Merge();
            worksheet.Cell(rowNumber + 13, 10).Value = deliveryEmployee.EmployeeName;
            SetFormatCell(ref worksheet, rowNumber + 13, 10, true, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center);

            MemoryStream memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }

        public byte[] GenerateExcelSalary(List<EmployeeInputDto> employee, List<ExportImport> exportImport)
        {

        }
    }           
}
