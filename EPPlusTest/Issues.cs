﻿using System;
using System.Diagnostics;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;

namespace EPPlusTest
{
    [TestClass]
    public class Issues
    {
        [TestMethod]
        public void Issue15052()
        {
            var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("test");
            ws.Cells["A1:A4"].Value = 1;
            ws.Cells["B1:B4"].Value = 2;

            ws.Cells[1, 1, 4, 1]
                        .Style.Numberformat.Format = "#,##0.00;[Red]-#,##0.00";

            ws.Cells[1, 2, 5, 2]
                                    .Style.Numberformat.Format = "#,##0;[Red]-#,##0";

            p.SaveAs(new FileInfo(@"c:\temp\style.xlsx"));
        }
        [TestMethod]
        public void Issue15041()
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Test");
                ws.Cells["A1"].Value = 202100083;
                ws.Cells["A1"].Style.Numberformat.Format = "00.00.00.000.0";
                Assert.AreEqual("02.02.10.008.3", ws.Cells["A1"].Text);
                ws.Dispose();
            }
        }
        [TestMethod]
        public void Issue15031()
        {
            var d = OfficeOpenXml.Utils.ConvertUtil.GetValueDouble(new TimeSpan(35, 59, 1));
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Test");
                ws.Cells["A1"].Value = d;
                ws.Cells["A1"].Style.Numberformat.Format = "[t]:mm:ss";
                ws.Dispose();
            }
        }
        [TestMethod]
        public void Issue15022()
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Test");
                ws.Cells.AutoFitColumns();
                ws.Cells["A1"].Style.Numberformat.Format = "0";
                ws.Cells.AutoFitColumns();
            }
        }
        [TestMethod]
        public void Issue15056()
        {
            var path = @"C:\temp\output.xlsx";
            var file = new FileInfo(path);
            file.Delete();
            using (var ep = new ExcelPackage(file))
            {
                var s = ep.Workbook.Worksheets.Add("test");
                s.Cells["A1:A2"].Formula = ""; // or null, or non-empty whitespace, with same result
                ep.Save();
            }

        }
        [Ignore]
        [TestMethod]
        public void Issue15058()
        {
            System.IO.FileInfo newFile = new System.IO.FileInfo(@"C:\Temp\output.xlsx");
            ExcelPackage excelP = new ExcelPackage(newFile);
            ExcelWorksheet ws = excelP.Workbook.Worksheets[1];
        }
        [Ignore]
        [TestMethod]
        public void Issue15063()
        {
            System.IO.FileInfo newFile = new System.IO.FileInfo(@"C:\Temp\bug\TableFormula.xlsx");
            ExcelPackage excelP = new ExcelPackage(newFile);
            ExcelWorksheet ws = excelP.Workbook.Worksheets[1];
            ws.Calculate();
        }
        [Ignore]        
        [TestMethod]
        public void Issue15112()
        {
            System.IO.FileInfo case1 = new System.IO.FileInfo(@"c:\temp\bug\src\src\DeleteRowIssue\Template.xlsx");
            var p = new ExcelPackage(case1);
            var first = p.Workbook.Worksheets[1];
            first.DeleteRow(5);
            p.SaveAs(new System.IO.FileInfo(@"c:\temp\bug\DeleteCol_case1.xlsx"));
            
            var case2 = new System.IO.FileInfo(@"c:\temp\bug\src2\DeleteRowIssue\Template.xlsx");
            p = new ExcelPackage(case2);
            first = p.Workbook.Worksheets[1];
            first.DeleteRow(5);
            p.SaveAs(new System.IO.FileInfo(@"c:\temp\bug\DeleteCol_case2.xlsx"));
        }

        [Ignore]
        [TestMethod]
        public void Issue15118()
        {
            using (var package = new OfficeOpenXml.ExcelPackage(new FileInfo(@"c:\temp\bugOutput.xlsx"), new FileInfo(@"c:\temp\bug\DeleteRowIssue\Template.xlsx")))
            {
                ExcelWorkbook workBook = package.Workbook;
                var worksheet = workBook.Worksheets[1];
                worksheet.Cells[9, 6, 9, 7].Merge = true;
                worksheet.Cells[9, 8].Merge = false;

                worksheet.DeleteRow(5);
                worksheet.DeleteColumn(5);
                worksheet.DeleteColumn(5);
                worksheet.DeleteColumn(5);
                worksheet.DeleteColumn(5);

                package.Save();
            }            
        }
        [Ignore]
        [TestMethod]
        public void Issue15109()
        {
            System.IO.FileInfo newFile = new System.IO.FileInfo(@"C:\Temp\bug\test01.xlsx");
            ExcelPackage excelP = new ExcelPackage(newFile);
            ExcelWorksheet ws = excelP.Workbook.Worksheets[1];
            Assert.AreEqual("A1:Z75",ws.Dimension.Address);
            excelP.Dispose();

            newFile = new System.IO.FileInfo(@"C:\Temp\bug\test02.xlsx");
            excelP = new ExcelPackage(newFile);
            ws = excelP.Workbook.Worksheets[1];
            Assert.AreEqual("A1:AF501", ws.Dimension.Address);
            excelP.Dispose();

            newFile = new System.IO.FileInfo(@"C:\Temp\bug\test03.xlsx");
            excelP = new ExcelPackage(newFile);
            ws = excelP.Workbook.Worksheets[1];
            Assert.AreEqual("A1:AD406", ws.Dimension.Address);
            excelP.Dispose();
        }
        [Ignore]
        [TestMethod]
        public void Issue15120()
        {
            var p=new ExcelPackage(new System.IO.FileInfo(@"C:\Temp\bug\pp.xlsx"));
            ExcelWorksheet ws = p.Workbook.Worksheets["tum_liste"];
            ExcelWorksheet wPvt = p.Workbook.Worksheets.Add("pvtSheet");
            var pvSh = wPvt.PivotTables.Add(wPvt.Cells["B5"], ws.Cells[ws.Dimension.Address.ToString()], "pvtS");

            //p.Save();
        }
        [TestMethod]
        public void Issue15113()
        {
            var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("t");
            ws.Cells["A1"].Value = " Performance Update";
            ws.Cells["A1:H1"].Merge = true;
            ws.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            ws.Cells["A1:H1"].Style.Font.Size = 14;
            ws.Cells["A1:H1"].Style.Font.Color.SetColor(Color.Red);
            ws.Cells["A1:H1"].Style.Font.Bold = true;
            p.SaveAs(new FileInfo(@"c:\temp\merge.xlsx"));
        }
        [TestMethod]
        public void Issue15123()
        {
            var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("t");
            using (var dt = new DataTable())
            {
                dt.Columns.Add("String", typeof(string));
                dt.Columns.Add("Int", typeof(int));
                dt.Columns.Add("Bool", typeof(bool));
                dt.Columns.Add("Double", typeof(double));
                dt.Columns.Add("Date", typeof(DateTime));

                var dr = dt.NewRow();
	                 dr[0] = "Row1";
	                 dr[1] = 1;
	                 dr[2] = true;
	                 dr[3] = 1.5;
                     dr[4] = new DateTime(2014, 12, 30);
	                 dt.Rows.Add(dr);
	  
	                 dr = dt.NewRow();
	                 dr[0] = "Row2";
	                 dr[1] = 2;
	                 dr[2] = false;
	                 dr[3] = 2.25;
                     dr[4] = new DateTime(2014, 12, 31);
	                 dt.Rows.Add(dr);
                
                ws.Cells["A1"].LoadFromDataTable(dt,true);
                ws.Cells["D2:D3"].Style.Numberformat.Format = "(* #,##0.00);_(* (#,##0.00);_(* \"-\"??_);(@)";
                
                ws.Cells["E2:E3"].Style.Numberformat.Format = "mm/dd/yyyy";
                ws.Cells.AutoFitColumns();
                Assert.AreNotEqual(ws.Cells[2, 5].Text,"");
            }            
        }
                    
        [TestMethod]
        public void Issue15128()
        {
            var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("t");
            ws.Cells["A1"].Value=1;
            ws.Cells["B1"].Value = 2;
            ws.Cells["B2"].Formula = "A1+$B$1";
            ws.Cells["C1"].Value = "Test";
            ws.Cells["A1:B2"].Copy(ws.Cells["C1"]);
            ws.Cells["B2"].Copy(ws.Cells["D1"]);
            p.SaveAs(new FileInfo(@"c:\temp\bug\copy.xlsx"));
        }

        [TestMethod]
        public void IssueMergedCells()
        {
            var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("t");
            ws.Cells["A1:A5,C1:C8"].Merge = true;
            ws.Cells["C1:C8"].Merge = false;
            ws.Cells["A1:A8"].Merge = false;
            p.Dispose();
        }

        [Ignore]
        [TestMethod]
        public void Issue14988()
        {
            var guid = Guid.NewGuid().ToString("N");
            using (var outputStream = new FileStream(@"C:\temp\" + guid + ".xlsx", FileMode.Create))
            {
                using (var inputStream = new FileStream(@"C:\temp\bug2.xlsx", FileMode.Open))
                {
                    using (var package = new ExcelPackage(outputStream, inputStream, "Test"))
                    {
                        //foreach (var names in package.Workbook.Names)
                        //{
                        //    if (names.Name.Equals("ReportTitle"))
                        //    {
                        //        //names.First().Value = "TestingValue";
                        //    }
                        //}
                        var ws= package.Workbook.Worksheets.Add("Test empty");
                        ws.Cells["A1"].Value = "Test";
                        package.Encryption.Password = "Test2";
                        package.Save();
                        //package.SaveAs(new FileInfo(@"c:\temp\test2.xlsx"));
                    }
                }
            }
        }
    }
}