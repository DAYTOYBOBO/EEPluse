﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml.Utils;
using OfficeOpenXml;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Security.Cryptography;

namespace EPPlusTest
{
    [TestClass]
    public class VBA
    {
        [TestMethod]
        public void Compression()
        {
            //Compression/Decompression
            string value = "#aaabcdefaaaaghijaaaaaklaaamnopqaaaaaaaaaaaarstuvwxyzaaa";
            
            byte[] compValue = CompoundDocument.CompressPart(Encoding.GetEncoding(1252).GetBytes(value));
            string decompValue = Encoding.GetEncoding(1252).GetString(CompoundDocument.DecompressPart(compValue));
            Assert.AreEqual(value, decompValue);

            value = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            
            compValue = CompoundDocument.CompressPart(Encoding.GetEncoding(1252).GetBytes(value));
            decompValue = Encoding.GetEncoding(1252).GetString(CompoundDocument.DecompressPart(compValue));
            Assert.AreEqual(value, decompValue);
        }
        [TestMethod]
        public void ReadVBA()
        {
            var package = new ExcelPackage(new FileInfo(@"c:\temp\report.xlsm"));
            File.WriteAllText(@"c:\temp\vba\modules\dir.txt", package.Workbook.VbaProject.CodePage + "," + package.Workbook.VbaProject.Constants + "," + package.Workbook.VbaProject.Description+ "," + package.Workbook.VbaProject.HelpContextID.ToString()+ "," + package.Workbook.VbaProject.HelpFile1+ "," + package.Workbook.VbaProject.HelpFile2+ "," + package.Workbook.VbaProject.Lcid.ToString()+ "," + package.Workbook.VbaProject.LcidInvoke.ToString()+ "," + package.Workbook.VbaProject.LibFlags.ToString()+ "," + package.Workbook.VbaProject.MajorVersion.ToString()+ "," + package.Workbook.VbaProject.MinorVersion.ToString()+ "," + package.Workbook.VbaProject.Name+ "," + package.Workbook.VbaProject.ProjectID + "," + package.Workbook.VbaProject.SystemKind.ToString() + "," + package.Workbook.VbaProject.Protection.HostProtected.ToString()+ "," + package.Workbook.VbaProject.Protection.UserProtected.ToString()+ "," + package.Workbook.VbaProject.Protection.VbeProtected.ToString()+ "," + package.Workbook.VbaProject.Protection.VisibilityState.ToString());
            foreach (var module in package.Workbook.VbaProject.Modules)
            {
                File.WriteAllText(string.Format(@"c:\temp\vba\modules\{0}.txt",module.Name),module.Code);
            }
            foreach (var r in package.Workbook.VbaProject.References)
            {
                File.WriteAllText(string.Format(@"c:\temp\vba\modules\{0}.txt", r.Name), r.Libid + " " +r.ReferenceRecordID.ToString());
            }

            List<X509Certificate2> ret = new List<X509Certificate2>();
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            package.Workbook.VbaProject.Signature.Certificate = store.Certificates[19];
            //package.Workbook.VbaProject.Protection.SetPassword("");
            package.SaveAs(new FileInfo(@"c:\temp\vbaSaved.xlsm"));
        }
        [TestMethod]
        public void WriteVBA()
        {
            var package = new ExcelPackage();
            package.Workbook.Worksheets.Add("Sheet1");
            package.Workbook.CreateVBAProject();
            package.Workbook.VbaProject.Modules["Sheet1"].Code += "\r\nPrivate Sub Worksheet_SelectionChange(ByVal Target As Range)\r\nMsgBox(\"Test of the VBA Feature!\")\r\nEnd Sub\r\n";
            package.Workbook.VbaProject.Modules["Sheet1"].Name = "Blad1";
            package.Workbook.CodeModule.Name = "DenHärArbetsboken";
            package.Workbook.Worksheets[1].Name = "FirstSheet";
            package.Workbook.CodeModule.Code += "\r\nPrivate Sub Workbook_Open()\r\nBlad1.Cells(1,1).Value = \"VBA test\"\r\nMsgBox \"VBA is running!\"\r\nEnd Sub";
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            package.Workbook.VbaProject.Signature.Certificate = store.Certificates[11];

            var m=package.Workbook.VbaProject.Modules.AddModule("Module1");         
            m.Code += "Public Sub Test(param1 as string)\r\n\r\nEnd sub\r\nPublic Function functest() As String\r\n\r\nEnd Function\r\n";
            var c = package.Workbook.VbaProject.Modules.AddClass("Class1", false);
            c.Code += "Private Sub Class_Initialize()\r\n\r\nEnd Sub\r\nPrivate Sub Class_Terminate()\r\n\r\nEnd Sub";
            var c2=package.Workbook.VbaProject.Modules.AddClass("Class2", true);
            c2.Code += "Private Sub Class_Initialize()\r\n\r\nEnd Sub\r\nPrivate Sub Class_Terminate()\r\n\r\nEnd Sub";
            
            package.Workbook.VbaProject.Protection.SetPassword("EPPlus");
            package.SaveAs(new FileInfo(@"c:\temp\vbaWrite.xlsm"));

        }
        [TestMethod]
        public void Resign()
        {
            var package = new ExcelPackage(new FileInfo(@"c:\temp\vbaWrite.xlsm"));
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            package.Workbook.VbaProject.Signature.Certificate = store.Certificates[11];
            package.SaveAs(new FileInfo(@"c:\temp\vbaWrite2.xlsm"));
        }
        [TestMethod]
        public void WriteLongVBAModule()
        {
            var package = new ExcelPackage();
            package.Workbook.Worksheets.Add("VBASetData");
            package.Workbook.CreateVBAProject();
            package.Workbook.CodeModule.Code = "Private Sub Workbook_Open()\r\nCreateData\r\nEnd Sub";
            var module=package.Workbook.VbaProject.Modules.AddModule("Code");

                StringBuilder code = new StringBuilder("Public Sub CreateData()\r\n");
            for (int row = 1; row < 30; row++)
            {
                for (int col = 1; col < 30; col++)
                {
                    code.AppendLine(string.Format("VBASetData.Cells({0},{1}).Value=\"Cell {2}\"", row, col, new ExcelAddressBase(row, col, row, col).Address));
                }                
            }
            code.AppendLine("End Sub");
            module.Code = code.ToString();

            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            package.Workbook.VbaProject.Signature.Certificate = store.Certificates[19];

            package.SaveAs(new FileInfo(@"c:\temp\vbaLong.xlsm"));
        }
    }
}
