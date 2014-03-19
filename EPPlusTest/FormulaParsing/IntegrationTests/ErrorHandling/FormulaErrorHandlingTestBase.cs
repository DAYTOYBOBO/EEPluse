﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;

namespace EPPlusTest.FormulaParsing.IntegrationTests
{
    [TestClass]
    public class FormulaErrorHandlingTestBase
    {
        protected ExcelPackage Package;
        protected ExcelWorksheet Worksheet;

        public void BaseInitialize()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var Package = new ExcelPackage(new FileInfo(Path.Combine(dir, "Workbooks", "FormulaTest.xlsx")));
            Worksheet = Package.Workbook.Worksheets["ValidateFormulas"];
            Package.Workbook.Calculate();
        }

        public void BaseCleanup()
        {
            Package.Dispose();
        }
    }
}
