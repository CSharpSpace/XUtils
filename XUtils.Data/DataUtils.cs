using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
namespace XUtils.Data
{
	public class DataUtils
	{
		public static DataTable ConvertToDataTable<T>(IList<T> objects)
		{
			List<PropertyInfo> properties = BusinessProperty.GetProperties(typeof(T));
			DataTable dataTable = new DataTable();
			foreach (PropertyInfo current in properties)
			{
				DataColumn dataColumn = new DataColumn();
				dataColumn.ColumnName = current.Name;
				dataColumn.DataType = current.PropertyType;
				dataColumn.DefaultValue = null;
				dataTable.Columns.Add(dataColumn);
			}
			foreach (object obj in objects)
			{
				DataRow dataRow = dataTable.NewRow();
				foreach (PropertyInfo current2 in properties)
				{
					object value = current2.GetValue(obj, null);
					dataRow[current2.Name] = value;
				}
				dataTable.Rows.Add(dataRow);
			}
			return dataTable;
		}
		public static DataTable ConvertToDataTable(IList objects, IList<PropertyInfo> properties)
		{
			DataTable dataTable = new DataTable();
			foreach (PropertyInfo current in properties)
			{
				DataColumn dataColumn = new DataColumn();
				dataColumn.ColumnName = current.Name;
				dataColumn.DataType = current.PropertyType;
				dataColumn.DefaultValue = null;
				dataTable.Columns.Add(dataColumn);
			}
			foreach (object current2 in objects)
			{
				DataRow dataRow = dataTable.NewRow();
				foreach (PropertyInfo current3 in properties)
				{
					object value = current3.GetValue(current2, null);
					dataRow[current3.Name] = value;
				}
				dataTable.Rows.Add(dataRow);
			}
			return dataTable;
		}
	}
}
