using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

    public static class ExcelFileGenerator
    {

        public static byte[] GenerateExcelFile(List<ExportImportProductDto> list)
        {
            var memoryStream = new MemoryStream();

            using var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            sheets.AppendChild(new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet 1"
            });

            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            ConvertClass convert = new ConvertClass();
            DataTable dataTable = convert.ConvertToDataTable(list);

            //char[] refrence = "ABCDEFGHIJKLMNOPQRSTUVWXYZAAABAC".ToCharArray();

            //foreach (DataRow item in dataTable.Rows)
            //{
            //    int productRows = 16;
            //    productRows += dataTable.Rows.IndexOf(item);

            //    Row row = new Row();
            //    for (int i = 0; i < item.ItemArray.Length; i++)
            //    {
            //        Cell cell = new Cell()
            //        {
            //            CellValue = new CellValue(item[i].ToString()),
            //            CellReference = refrence[i].ToString() + productRows,
            //            DataType = CellValues.String
            //        };
            //        row.AppendChild(cell);
            //    }
            //    sheetData?.AppendChild(row);
            //}            

            //foreach (DataRow item in dataTable.Rows)
            //{
            //    int productRows = 16;
            //    productRows += dataTable.Rows.IndexOf(item);

            //    Row row = new Row();
            //    for (int i = 0; i < item.ItemArray.Length; i++)
            //    {
            //        Cell cell = new Cell()
            //        {
            //            CellValue = new CellValue(item[i].ToString()),
            //            CellReference = "A1",
            //            DataType = CellValues.String
            //        };
            //        row.AppendChild(cell);
            //    }
            //    sheetData?.AppendChild(row);
            //}
            Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == 16);
            if (row == null)
            {
                row = new Row { 
                    RowIndex = 16
                };

                row.AppendChild(new Cell
                {
                    CellReference = "B16",
                    CellValue = new CellValue("Abp Framework"),
                    DataType = CellValues.String
                });

                sheetData?.AppendChild(row);                
            }

            

            //sheetData?.AppendChild(row);

            //var row2 = new Row();
            //row2.AppendChild(
            //    new Cell
            //    {
            //        CellValue = new CellValue("Nguyễn Tấn Phát"),
            //        DataType = CellValues.String
            //    });
            //sheetData?.AppendChild(row2);

            //var row3 = new Row();
            //row3.AppendChild(
            //    new Cell
            //    {
            //        CellValue = new CellValue("WEB APPLICATION FRAMEWORK"),
            //        DataType = CellValues.String
            //    });
            //sheetData?.AppendChild(row3);

            document.Save();
            document.Close();

            return memoryStream.ToArray();
        }



        public static byte[] ExcelGenerator(List<ExportImportProductDto> list)
        {
            string filePath = @"E:\Documents\GitHub\End_Of_Year\aspnet-core\src\Nguyen_Tan_Phat_Project.Web.Host\wwwroot\ExcelTemplate\PHIEU XUAT KHO.xlsx";
            byte[] bytes = File.ReadAllBytes(filePath);
            var memoryStream = new MemoryStream();
            memoryStream.Write(bytes, 0, bytes.Length);

            using var spreadsheet = SpreadsheetDocument.Open(memoryStream, true);

            WorkbookPart workbookPart = spreadsheet.WorkbookPart;

            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            char[] refrence = "ABCDEFGHIJKLMNOPQRSTUVWXYZAAABAC".ToCharArray();

            int exportCodeRows = 10;
            int exportCustomerRows = 12;
            int exportAddressRows = 13;
            int exportStorageRows= 14;

            ConvertClass convert = new ConvertClass();
            DataTable dataTable = convert.ConvertToDataTable(list);


            foreach (DataRow item in dataTable.Rows)
            {
                int productRows = 16;
                productRows += dataTable.Rows.IndexOf(item);

                Row row = new Row();
                for (int i = 0; i < item.ItemArray.Length; i++)
                {
                    Cell cell = new Cell()
                    { 
                        CellValue = new CellValue(item[i].ToString()),
                        CellReference = refrence[i].ToString() + productRows,
                        DataType = CellValues.String
                    };
                    row.Append(cell);
                }
                sheetData.Append(row);
            }
            workbookPart.Workbook.Save();

            return memoryStream.ToArray();
        }
    }
}
