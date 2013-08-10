﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using System.IO;

namespace EPPlusTest
{
    [TestClass]
    public class Encrypt
    {
        [TestMethod]
        public void ReadWriteEncrypt()
        {
            using (ExcelPackage pck = new ExcelPackage(new FileInfo(@"Test\Drawing.xlsx"), true))   
            {
                pck.Encryption.Password = "EPPlus";
                pck.Encryption.Algorithm = EncryptionAlgorithm.AES192;
                pck.Workbook.Protection.SetPassword("test");
                pck.Workbook.Protection.LockStructure = true;
                pck.Workbook.Protection.LockWindows = true;

                pck.SaveAs(new FileInfo(@"Test\DrawingEncr.xlsx"));                
            }

            using (ExcelPackage pck = new ExcelPackage(new FileInfo(@"Test\DrawingEncr.xlsx"), true, "EPPlus"))            
            {
                pck.Encryption.IsEncrypted = false;
                pck.SaveAs(new FileInfo(@"Test\DrawingNotEncr.xlsx"));
            }

            FileStream fs = new FileStream(@"Test\DrawingEncr.xlsx", FileMode.Open, FileAccess.ReadWrite);
            using (ExcelPackage pck = new ExcelPackage(fs, "EPPlus"))
            {
                pck.Encryption.IsEncrypted = false;
                pck.SaveAs(new FileInfo(@"Test\DrawingNotEncr.xlsx"));
            }

        }
        [TestMethod]
        public void WriteEncrypt()
        {
            ExcelPackage package = new ExcelPackage();
            //Load the sheet with one string column, one date column and a few random numbers.
            var ws = package.Workbook.Worksheets.Add("First line test");

            ws.Cells[1, 1].Value = "1; 1";
            ws.Cells[2, 1].Value = "2; 1";
            ws.Cells[1, 2].Value = "1; 2";
            ws.Cells[2, 2].Value = "2; 2";

            ws.Row(1).Style.Font.Bold = true;
            ws.Column(1).Style.Font.Bold = true;

            //package.Encryption.Algorithm = EncryptionAlgorithm.AES256;
            //package.SaveAs(new FileInfo(@"c:\temp\encrTest.xlsx"), "ABxsw23edc");
            package.Encryption.Password = "test";
            package.Encryption.IsEncrypted = true;
            package.SaveAs(new FileInfo(@"c:\temp\encrTest.xlsx"));
        }
        [TestMethod]
        public void WriteProtect()
        {
            ExcelPackage package = new ExcelPackage(new FileInfo(@"c:\temp\workbookprot2.xlsx"), "");
            //Load the sheet with one string column, one date column and a few random numbers.
            //package.Workbook.Protection.LockWindows = true;
            //package.Encryption.IsEncrypted = true;
            package.Workbook.Protection.SetPassword("t");
            package.Workbook.Protection.LockStructure = true;
            package.Workbook.View.Left = 585;
            package.Workbook.View.Top = 150;

            package.Workbook.View.Width = 17310;
            package.Workbook.View.Height = 38055;
            var ws = package.Workbook.Worksheets.Add("First line test");

            ws.Cells[1, 1].Value = "1; 1";
            ws.Cells[2, 1].Value = "2; 1";
            ws.Cells[1, 2].Value = "1; 2";
            ws.Cells[2, 2].Value = "2; 2";

            package.SaveAs(new FileInfo(@"c:\temp\workbookprot2.xlsx"));

        }
        [TestMethod]
        public void DecrypTest()
        {
            var p = new ExcelPackage(new FileInfo(@"c:\temp\encr.xlsx"), "test");

            var n = p.Workbook.Worksheets[1].Name;
            p.Encryption.Password = null;
            p.SaveAs(new FileInfo(@"c:\temp\encrNew.xlsx"));
        
        }
        [TestMethod]
        public void EncrypTest()
        {
            var f = new FileInfo(@"c:\temp\encrwrite.xlsx");
            if (f.Exists)
            {
                f.Delete();
            }
            var p = new ExcelPackage(f);
            
            p.Workbook.Protection.SetPassword("");
            p.Workbook.Protection.LockStructure = true;
            p.Encryption.Version = EncryptionVersion.Agile;

            var ws = p.Workbook.Worksheets.Add("Sheet1");
            for (int r = 1; r < 1000; r++)
            {
                ws.Cells[r, 1].Value = r;
            }
            p.Save();
        }
    }
}
