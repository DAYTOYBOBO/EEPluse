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
 *******************************************************************************
 * Jan Källman		Added		12-APR-2012
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeOpenXml.VBA
{
    public class ExcelVBACollectionBase<T> : IEnumerable<T>
    {
        internal protected List<T> _list=new List<T>();
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        public T this [string Name]
        {
            get
            {
                return _list.Find((f) => f.GetType().GetProperty("Name").GetValue(f, null).ToString().ToLower() == Name.ToLower());
            }
        }
        public T this[int Index]
        {
            get
            {
                return _list[Index];
            }
        }
        public int Count
        {
            get { return _list.Count; }
        }
        public bool Exists(string Name)
        {
            return _list.Exists((f) => f.GetType().GetProperty("Name").GetValue(f, null).ToString().ToLower() == Name.ToLower());
        }
        public void Remove(T Item)
        {
            _list.Remove(Item);
        }
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
        internal void Clear()
        {
            _list.Clear();
        }
    }
    public class ExcelVbaModuleCollection : ExcelVBACollectionBase<ExcelVBAModule>
    {
        ExcelVbaProject _project;
        internal ExcelVbaModuleCollection (ExcelVbaProject project)
	    {
            _project=project;
	    }
        internal void Add(ExcelVBAModule Item)
        {
            _list.Add(Item);
        }
        public ExcelVBAModule AddModule(string Name)
        {
            var m = new ExcelVBAModule();
            m.Name = Name;
            m.Type = eModuleType.Module;
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_Name", Value = Name, DataType = eAttributeDataType.String });
            m.Type = eModuleType.Module;
            _list.Add(m);
            return m;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name">The name of the class</param>
        /// <param name="Exposed">Private or Public not createble</param>
        /// <returns></returns>
        public ExcelVBAModule AddClass(string Name, bool Exposed)
        {
            var m = new ExcelVBAModule();
            m.Name = Name;            
            m.Type = eModuleType.Class;
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_Name", Value = Name, DataType = eAttributeDataType.String });
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_Base", Value = "0{FCFB3D2A-A0FA-1068-A738-08002B3371B5}", DataType = eAttributeDataType.String });
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_GlobalNameSpace", Value = "False", DataType = eAttributeDataType.NonString });
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_Creatable", Value = "False", DataType = eAttributeDataType.NonString });
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_PredeclaredId", Value = "False", DataType = eAttributeDataType.NonString });
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_Exposed", Value = Exposed ? "True" : "False", DataType = eAttributeDataType.NonString });
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_TemplateDerived", Value = "False", DataType = eAttributeDataType.NonString });
            m.Attributes._list.Add(new ExcelVbaModuleAttribute() { Name = "VB_Customizable", Value = "False", DataType = eAttributeDataType.NonString });

            //m.Code = _project.GetBlankClassModule(Name, Exposed);
            m.Private = !Exposed;
            //m.ClassID=
            _list.Add(m);
            return m;
        }
    }
    public class ExcelVbaReferenceCollection : ExcelVBACollectionBase<ExcelVbaReference>
    {
        public ExcelVbaReferenceCollection()
        {

        }
        public void Add(ExcelVbaReference Item)
        {
            _list.Add(Item);
        }
    }
    public class ExcelVbaModuleAttributesCollection : ExcelVBACollectionBase<ExcelVbaModuleAttribute>
    {
        internal string GetAttributeText()
        {
            StringBuilder sb=new StringBuilder();

            foreach (var attr in this)
            {
                sb.AppendFormat("Attribute {0} = {1}\r\n", attr.Name, attr.DataType==eAttributeDataType.String ? "\"" + attr.Value + "\"" : attr.Value);
            }
            return sb.ToString();
        }
    }
}
