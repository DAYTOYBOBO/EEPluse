﻿/* Copyright (C) 2011  Jan Källman
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
 *******************************************************************************
 * Mats Alm   		                Added		                2013-12-03
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml.FormulaParsing.ExcelUtilities;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using OfficeOpenXml.FormulaParsing.Exceptions;

namespace OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup
{
    public abstract class LookupFunction : ExcelFunction
    {
        private readonly ValueMatcher _valueMatcher;

        public LookupFunction()
            : this(new ValueMatcher())
        {

        }

        public LookupFunction(ValueMatcher valueMatcher)
        {
            _valueMatcher = valueMatcher;
        }

        public override bool IsLookupFuction
        {
            get
            {
                return true;
            }
        }

        protected int IsMatch(object o1, object o2)
        {
            return _valueMatcher.IsMatch(o1, o2);
        }

        protected LookupDirection GetLookupDirection(RangeAddress rangeAddress)
        {
            var nRows = rangeAddress.ToRow - rangeAddress.FromRow;
            var nCols = rangeAddress.ToCol - rangeAddress.FromCol;
            return nCols > nRows ? LookupDirection.Horizontal : LookupDirection.Vertical;
        }

        protected CompileResult Lookup(LookupNavigator navigator, LookupArguments lookupArgs)
        {
            object lastValue = null;
            object lastLookupValue = null;
            int? lastMatchResult = null;
            do
            {
                var matchResult = IsMatch(navigator.CurrentValue, lookupArgs.SearchedValue);
                if (matchResult == 0)
                {
                    return CreateResult(navigator.GetLookupValue(), DataType.String);
                }
                if (lookupArgs.RangeLookup)
                {
                    if (lastValue != null && matchResult > 0 && lastMatchResult < 0)
                    {
                        return CreateResult(lastLookupValue, DataType.String);
                    }
                    lastMatchResult = matchResult;
                    lastValue = navigator.CurrentValue;
                    lastLookupValue = navigator.GetLookupValue();
                }
            }
            while (navigator.MoveNext());

            throw new ExcelFunctionException("Lookupfunction failed to lookup value", ExcelErrorCodes.NoValueAvaliable);
        }
    }
}
