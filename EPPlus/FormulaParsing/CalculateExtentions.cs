﻿/*******************************************************************************
 * You may amend and distribute as you like, but don't remove this header!
 *
 * EPPlus provides server-side generation of Excel 2007/2010 spreadsheets.
 * See http://www.codeplex.com/EPPlus for details.
 *
 * Copyright (C) 2011  Jan Källman
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.

 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  
 * See the GNU Lesser General Public License for more details.
 *
 * The GNU Lesser General Public License can be viewed at http://www.opensource.org/licenses/lgpl-license.php
 * If you unfamiliar with this license or have questions about it, here is an http://www.gnu.org/licenses/gpl-faq.html
 *
 * All code and executables are provided "as is" with no warranty either express or implied. 
 * The author accepts no liability for any damage or loss of business that this product may cause.
 *
 * Code change notes:
 * 
 * Author							Change						Date
 * ******************************************************************************
 * Jan Källman                      Added                       2012-03-04  
 *******************************************************************************/
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using OfficeOpenXml.FormulaParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace OfficeOpenXml.Calculation
{
    public static class CalculationExtension
    {
        public static void Calculate(this ExcelWorkbook Workbook)
        {
            var dc = CalculationChain.GetChain(Workbook);
            var parser = new FormulaParser(new EpplusExcelDataProvider(Workbook._package));
            //TODO: Add calculation here
            foreach (var ix in dc.CalcOrder)
            {
                var item = dc.list[ix];
                var v = parser.ParseAt(ExcelAddressBase.GetAddress(item.Row, item.Column));
                item.ws.SetFormula(item.Row, item.Column, v);
            }
        }

        private static object GetFormulaValue(FormulaCell item)
        {
            throw new NotImplementedException();
        }
        public static void Calculate(this ExcelWorksheet Worksheet)
        {
            var parser = new FormulaParser(new EpplusExcelDataProvider(Worksheet._package));
            var dc = CalculationChain.GetChain(Worksheet);
            foreach (var ix in dc.CalcOrder)
            {
                var item = dc.list[ix];
                var v = parser.ParseAt(ExcelAddressBase.GetAddress(item.Row, item.Column));
                item.ws.SetFormula(item.Row, item.Column, v);
            }
        }
        public static void Calculate(this ExcelRangeBase Range)
        {
            var parser = new FormulaParser(new EpplusExcelDataProvider(Range.Worksheet._package));
            var dc = CalculationChain.GetChain(Range);
            foreach (var ix in dc.CalcOrder)
            {
                var item = dc.list[ix];
                var v = parser.ParseAt(ExcelAddressBase.GetAddress(item.Row, item.Column));
                item.ws.SetFormula(item.Row, item.Column, v);
            }
        }
    }
}
