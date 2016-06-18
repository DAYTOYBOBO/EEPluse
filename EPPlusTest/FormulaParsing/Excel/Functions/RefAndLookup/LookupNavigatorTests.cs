﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using Rhino.Mocks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using EPPlusTest.FormulaParsing.TestHelpers;
using OfficeOpenXml.FormulaParsing;
using OfficeOpenXml.FormulaParsing.ExcelUtilities;

namespace EPPlusTest.Excel.Functions.RefAndLookup
{
    [TestClass]
    public class LookupNavigatorTests
    {
        const string WorksheetName = "";
        private LookupArguments GetArgs(params object[] args)
        {
            var lArgs = FunctionsHelper.CreateArgs(args);
            return new LookupArguments(lArgs, ParsingContext.Create());
        }

        private ParsingContext GetContext(ExcelDataProvider provider)
        {
            var ctx = ParsingContext.Create();
            ctx.Scopes.NewScope(new RangeAddress(){Worksheet = WorksheetName, FromCol = 1, FromRow = 1});
            ctx.ExcelDataProvider = provider;
            return ctx;
        }

        //[TestMethod]
        //public void NavigatorShouldEvaluateFormula()
        //{
        //    var provider = MockRepository.GenerateStub<ExcelDataProvider>();
        //    provider.Stub(x => x.GetCellValue(WorksheetName,0, 0)).Return(new ExcelCell(3);
        //    provider.Stub(x => x.GetCellValue(WorksheetName,1, 0)).Return("B5");
        //    var args = GetArgs(4, "A1:B2", 1);
        //    var context = GetContext(provider);
        //    var parser = MockRepository.GenerateMock<FormulaParser>(provider);
        //    context.Parser = parser;
        //    var navigator = new LookupNavigator(LookupDirection.Vertical, args, context);
        //    navigator.MoveNext();
        //    parser.AssertWasCalled(x => x.Parse("B5"));
        //}

        [TestMethod]
        public void CurrentValueShouldBeFirstCell()
        {
            var provider = MockRepository.GenerateStub<ExcelDataProvider>();
            provider.Stub(x => x.GetCellValue(WorksheetName,1, 1)).Return(3);
            provider.Stub(x => x.GetCellValue(WorksheetName,2, 1)).Return(4);
            var args = GetArgs(3, "A1:B2", 1);
            var navigator = LookupNavigatorFactory.Create(LookupDirection.Vertical, args, GetContext(provider));
            Assert.AreEqual(3, navigator.CurrentValue);
        }

        [TestMethod]
        public void MoveNextShouldReturnFalseIfLastCell()
        {
            var provider = MockRepository.GenerateStub<ExcelDataProvider>();
            provider.Stub(x => x.GetCellValue(WorksheetName,1, 1)).Return(3);
            provider.Stub(x => x.GetCellValue(WorksheetName,2, 1)).Return(4);
            var args = GetArgs(3, "A1:B1", 1);
            var navigator = LookupNavigatorFactory.Create(LookupDirection.Vertical, args, GetContext(provider));
            Assert.IsFalse(navigator.MoveNext());
        }

        [TestMethod]
        public void HasNextShouldBeTrueIfNotLastCell()
        {
            var provider = MockRepository.GenerateStub<ExcelDataProvider>();
            provider.Stub(x => x.GetDimensionEnd(Arg<string>.Is.Anything)).Return(new ExcelCellAddress(5, 5));
            provider.Stub(x => x.GetCellValue(WorksheetName,1, 1)).Return(3);
            provider.Stub(x => x.GetCellValue(WorksheetName,2, 1)).Return(4);
            var args = GetArgs(3, "A1:B2", 1);
            var navigator = LookupNavigatorFactory.Create(LookupDirection.Vertical, args, GetContext(provider));
            Assert.IsTrue(navigator.MoveNext());
        }

        [TestMethod]
        public void MoveNextShouldNavigateVertically()
        {
            var provider = MockRepository.GenerateStub<ExcelDataProvider>();
            provider.Stub(x => x.GetCellValue(WorksheetName,1, 1)).Return(3);
            provider.Stub(x => x.GetCellValue(WorksheetName,2, 1)).Return(4);
            var args = GetArgs(6, "A1:B2", 1);
            var navigator = LookupNavigatorFactory.Create(LookupDirection.Vertical, args, GetContext(provider));
            navigator.MoveNext();
            Assert.AreEqual(4, navigator.CurrentValue);
        }

        [TestMethod]
        public void MoveNextShouldIncreaseIndex()
        {
            var provider = MockRepository.GenerateStub<ExcelDataProvider>();
            provider.Stub(x => x.GetDimensionEnd(Arg<string>.Is.Anything)).Return(new ExcelCellAddress(5, 5));
            provider.Stub(x => x.GetCellValue(WorksheetName, 1, 1)).Return(3);
            provider.Stub(x => x.GetCellValue(WorksheetName, 1, 2)).Return(4);
            var args = GetArgs(6, "A1:B2", 1);
            var navigator = LookupNavigatorFactory.Create(LookupDirection.Vertical, args, GetContext(provider));
            Assert.AreEqual(0, navigator.Index);
            navigator.MoveNext();
            Assert.AreEqual(1, navigator.Index);
        }

        [TestMethod]
        public void GetLookupValueShouldReturnCorrespondingValue()
        {
            var provider = MockRepository.GenerateStub<ExcelDataProvider>();
            provider.Stub(x => x.GetDimensionEnd(Arg<string>.Is.Anything)).Return(new ExcelCellAddress(5, 5));
            provider.Stub(x => x.GetCellValue(WorksheetName, 1, 1)).Return(3);
            provider.Stub(x => x.GetCellValue(WorksheetName, 1, 2)).Return(4);
            var args = GetArgs(6, "A1:B2", 2);
            var navigator = LookupNavigatorFactory.Create(LookupDirection.Vertical, args, GetContext(provider));
            Assert.AreEqual(4, navigator.GetLookupValue());
        }

        [TestMethod]
        public void GetLookupValueShouldReturnCorrespondingValueWithOffset()
        {
            var provider = MockRepository.GenerateStub<ExcelDataProvider>();
            provider.Stub(x => x.GetDimensionEnd(Arg<string>.Is.Anything)).Return(new ExcelCellAddress(5, 5));
            provider.Stub(x => x.GetCellValue(WorksheetName, 1, 1)).Return(3);
            provider.Stub(x => x.GetCellValue(WorksheetName, 3, 3)).Return(4);
            var args = new LookupArguments(3, "A1:A4", 3, 2, false,null);
            var navigator = LookupNavigatorFactory.Create(LookupDirection.Vertical, args, GetContext(provider));
            Assert.AreEqual(4, navigator.GetLookupValue());
        }
    }
}
