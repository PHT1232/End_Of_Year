using DocumentFormat.OpenXml;
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
        public Stylesheet InitStyleSheet(ref Stylesheet stylesheet)
        {
            //var stylesheet = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            //stylesheet.AddNamespaceDeclaration("mc", "http: //schemas.openxmlformats.org/markup-compatibility/2006");
            //stylesheet.AddNamespaceDeclaration("x14ac", "http: //schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            // create collections for fonts, fills, cellFormats, ...
            var fonts = new Fonts() { Count = 1U, KnownFonts = true };

            //var fills = new Fills() { Count = 5U };
            var cellFormats = new CellFormats() { Count = 4U };

            // create a font: bold, red, calibr
            Font font = new Font();
            font.Append(new FontSize() { Val = 11D });
            font.Append(new FontName() { Val = "Times New Roman" });
            font.Append(new FontFamilyNumbering() { Val = 2 });
            font.Append(new FontScheme() { Val = FontSchemeValues.Minor });
            font.Append(new Bold());
            // add the created font to the fonts collection
            // since this is the first added font it will gain the id 1U
            fonts.Append(font);

            // create a background: green
            //Fill fill = new Fill();
            //var patternFill = new PatternFill() { PatternType = PatternValues.Solid };
            //patternFill.Append(new ForegroundColor() { Rgb = "00ff00" });
            //patternFill.Append(new BackgroundColor() { Indexed = 64U });
            //fill.Append(patternFill);
            //fills.Append(fill);

            // create a cell format (combining font and background)
            // the first added font/fill/... has the id 0. The second 1,...
            cellFormats.AppendChild(new CellFormat() { FontId = 0U });

            // add the new collections to the stylesheet
            stylesheet.Append(fonts);
            //stylesheet.Append(fills);
            stylesheet.Append(cellFormats);

            return stylesheet;
        }

        public Border GenerateBorder()
        {
            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color1 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color1);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color2 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color2);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color3);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color4);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            return border2;
        }
    }

    public static class ExcelFileGenerator
    {

        public static byte[] GenerateExcelFile(List<ExportImportProductDto> list)
        {
            string filePath = @"E:\Documents\GitHub\End_Of_Year\aspnet-core\src\Nguyen_Tan_Phat_Project.Web.Host\wwwroot\ExcelTemplate\phieu xuat kho sua.xlsx";
            byte[] bytes = File.ReadAllBytes(filePath);
            var memoryStream = new MemoryStream();
            memoryStream.Write(bytes, 0, bytes.Length);

            char productCodeReferenceCell = 'B';
            char productCodeReferenceCell2 = 'D';

            ConvertClass convert = new ConvertClass();
            DataTable dataTable = convert.ConvertToDataTable(list);

            using var document = SpreadsheetDocument.Open(memoryStream, true);
            //using var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);
            //var workbookPart = document.AddWorkbookPart();
            //workbookPart.Workbook = new Workbook();

            //var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            //worksheetPart.Worksheet = new Worksheet(new SheetData());

            //var sheets = workbookPart.Workbook.AppendChild(new Sheets());

            //sheets.AppendChild(new Sheet
            //{
            //    Id = workbookPart.GetIdOfPart(worksheetPart),
            //    SheetId = 1,
            //    Name = "Sheet 1"
            //});

            WorkbookPart workbookPart = document.WorkbookPart;

            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();

            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            //var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

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

            //for (int i = 16; i < 20; i++)
            //{
            //    Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == i);
            //    if (row == null)
            //    {
            //        row = new Row
            //        {
            //            RowIndex = (uint)i,
            //        };

            //        row.AppendChild(new Cell
            //        {
            //            CellReference = "B" + i,
            //            CellValue = new CellValue(dataTable.Rows[0].ToString()),
            //            DataType = CellValues.String
            //        });

            //        row.AppendChild(new Cell
            //        {
            //            CellReference = "C" + i,
            //            CellValue = new CellValue(dataTable.Rows[0].ToString()),
            //            DataType = CellValues.String
            //        });
            //        row.AppendChild(new Cell
            //        {
            //            CellReference = "D" + i,
            //            CellValue = new CellValue(dataTable.Rows[0].ToString()),
            //            DataType = CellValues.String
            //        });
            //        row.AppendChild(new Cell
            //        {
            //            CellReference = "E" + i,
            //            CellValue = new CellValue(dataTable.Rows[0].ToString()),
            //            DataType = CellValues.String
            //        });
            //        row.AppendChild(new Cell
            //        {
            //            CellReference = "F" + i,
            //            CellValue = new CellValue(dataTable.Rows[0].ToString()),
            //            DataType = CellValues.String
            //        });

            //        sheetData?.AppendChild(row);
            //    }
            //}

            char STT = 'A';
            char productNameReferenceCell = 'E';
            char productNameReferenceCell2 = 'I';
            char productUnit = 'J';
            char productQuantity = 'K';
            string productPrice = "L";
            string productTotal = "M";

            int exportCodeRows = 10;
            int exportCustomerRows = 12;
            int exportAddressRows = 13;
            int exportStorageRows = 14;


            int STTNumber = 0;
            int productRows = 13;

            foreach (DataRow item in dataTable.Rows)
            {
                productRows += 1;
                STTNumber += 1;

                Row row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == productRows);
                if (row == null)
                {
                    row = new Row
                    {
                        RowIndex = ((uint)productRows)
                    };

                    row.AppendChild(new Cell
                    {
                        CellReference = STT.ToString() + productRows,
                        CellValue = new CellValue(STTNumber),
                        DataType = CellValues.String,
                    });

                    row.AppendChild(new Cell
                    {
                        CellReference = productCodeReferenceCell.ToString() + productRows,
                        CellValue = new CellValue(item[0].ToString()),
                        DataType = CellValues.String,
                    });

                    row.AppendChild(new Cell
                    {
                        CellReference = productNameReferenceCell.ToString() + productRows,
                        CellValue = new CellValue(item[1].ToString()),
                        DataType = CellValues.String,
                    });

                    row.AppendChild(new Cell
                    {
                        CellReference = productUnit.ToString() + productRows,
                        CellValue = new CellValue(item[2].ToString()),
                        DataType = CellValues.String,
                    });

                    row.AppendChild(new Cell
                    {
                        CellReference = productQuantity.ToString() + productRows,
                        CellValue = new CellValue(item[3].ToString()),
                        DataType = CellValues.String,
                    });

                    row.AppendChild(new Cell
                    {
                        CellReference = productPrice.ToString() + productRows,
                        CellValue = new CellValue(item[4].ToString()),
                        DataType = CellValues.String,
                    });

                    row.AppendChild(new Cell
                    {
                        CellReference = productTotal.ToString() + productRows,
                        CellValue = new CellValue(item[5].ToString()),
                        DataType = CellValues.String,
                    });

                    sheetData?.AppendChild(row);
                }
            }

            document.Save();
            document.Close();

            return memoryStream.ToArray();
        }
    }           
}
