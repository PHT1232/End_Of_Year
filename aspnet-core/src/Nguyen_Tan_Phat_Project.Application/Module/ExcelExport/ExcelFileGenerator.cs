using ClosedXML.Excel;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Data;
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

    public class FormatCell
    {
        public void AddBorderToCell()
        {

        }
    }

    public class ExcelFileGenerator
    {
        public byte[] GenerateExcelFileForExportImport(List<ExportImportProductDto> list, ExportImport exportImport, Customer customer, Employee employee)
        {
            XLWorkbook workbook = new XLWorkbook("E:\\Documents\\GitHub\\End_Of_Year\\aspnet-core\\src\\Nguyen_Tan_Phat_Project.Web.Host\\wwwroot\\ExcelTemplate\\phieu xuat kho sua.xlsx");
            IXLWorksheet worksheet = workbook.Worksheet("Sheet1");

            int rowNumber = 13, STT = 0;

            worksheet.Cell(6, 5).Value = "Ngày " + exportImport.CreationTime.Day + " tháng " + exportImport.CreationTime.Month + " năm " + exportImport.CreationTime.Year;
            worksheet.Cell(6, 5).Style.Font.FontSize = 12;
            worksheet.Cell(6, 5).Style.Font.FontName = "Times New Roman";
            worksheet.Cell(6, 5).Style.Font.SetBold(true);

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

            worksheet.Cell(11, 4).Value = exportImport.Storage.StorageName;
            worksheet.Cell(11, 4).Style.Font.FontSize = 12;
            worksheet.Cell(11, 4).Style.Font.FontName = "Times New Roman";

            foreach (var product in list)
            {
                rowNumber++;
                STT++;
                IXLRange range = worksheet.Range("B" + rowNumber + ":" + "D" + rowNumber);
                IXLRange range2 = worksheet.Range("E" + rowNumber + ":" + "I" + rowNumber);
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
                worksheet.Cell(rowNumber, 9).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(rowNumber, 10).Value = product.Unit;
                worksheet.Cell(rowNumber, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 10).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 10).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 11).Value = product.Quantity.ToString();
                worksheet.Cell(rowNumber, 11).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 11).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 11).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 12).Value = product.Price.ToString();
                worksheet.Cell(rowNumber, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 12).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 12).Style.Font.FontName = "Times New Roman";

                worksheet.Cell(rowNumber, 13).Value = product.FinalPrice.ToString();
                worksheet.Cell(rowNumber, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(rowNumber, 13).Style.Font.FontSize = 11;
                worksheet.Cell(rowNumber, 13).Style.Font.FontName = "Times New Roman";
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

            MemoryStream memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;
            return memoryStream.ToArray();
        }
    }           
}
